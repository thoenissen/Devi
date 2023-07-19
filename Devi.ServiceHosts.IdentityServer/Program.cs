using Devi.ServiceHosts.IdentityServer.Data;
using Devi.ServiceHosts.IdentityServer.Services;

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using OpenSearch.Net;

using Serilog;
using Serilog.Sinks.OpenSearch;

namespace Devi.ServiceHosts.IdentityServer;

/// <summary>
/// Main class
/// </summary>
public class Program
{
    /// <summary>
    /// Main method
    /// </summary>
    /// <param name="args">Arguments</param>
    public static void Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext()
                                                           .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        var openSearchUrl = Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_URL");
        var environment = Environment.GetEnvironmentVariable("DEVI_ENVIRONMENT");

        if (string.IsNullOrEmpty(openSearchUrl) == false
         && string.IsNullOrEmpty(environment) == false)
        {
            Func<ConnectionConfiguration, ConnectionConfiguration> modifyConnectionSettings = null;

            var user = Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_USER");

            if (string.IsNullOrWhiteSpace(user) == false)
            {
                modifyConnectionSettings = obj =>
                                           {
                                               obj.BasicAuthentication(user, Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_PASSWORD"));

                                               // HACK / TODO - Create real certificate
                                               obj.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
                                               return obj;
                                           };
            }

            loggerConfiguration.WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(openSearchUrl))
                                                   {
                                                       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.OSv2,
                                                       AutoRegisterTemplate = true,
                                                       MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                                                       IndexFormat = $"devi-{environment}-{DateTime.Now:yyyy-MM}",
                                                       ModifyConnectionSettings = modifyConnectionSettings
                                                   });
        }

        loggerConfiguration.Enrich.WithProperty("ServiceHost", "Devi.ServiceHosts.IdentityServer");

        Log.Logger = loggerConfiguration.CreateBootstrapLogger();

        Log.Information("Starting up");

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc
                                                 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                                                 .Enrich.FromLogContext()
                                                 .ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddSingleton<ICustomTokenRequestValidator, CustomTokenRequestValidator>();

            builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost);

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer());

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services
                   .AddIdentityServer(options =>
                                      {
                                          options.Events.RaiseErrorEvents = true;
                                          options.Events.RaiseInformationEvents = true;
                                          options.Events.RaiseFailureEvents = true;
                                          options.Events.RaiseSuccessEvents = true;
                                          options.EmitStaticAudienceClaim = true;
                                      })
                   .AddInMemoryIdentityResources(new IdentityResource[]
                                                 {
                                                     new IdentityResources.OpenId(),
                                                     new IdentityResources.Profile()
                                                 })
                   .AddInMemoryApiScopes(new ApiScope[]
                                         {
                                             new("api_public_v1"),
                                             new("api_internal_v1"),
                                         })
                   .AddInMemoryClients(new Client[]
                                       {
                                           new()
                                           {
                                               ClientId = Environment.GetEnvironmentVariable("DEVI_WEBAPI_CLIENT_ID"),
                                               ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("DEVI_WEBAPI_CLIENT_SECRET").Sha256()) },
                                               AllowedGrantTypes = GrantTypes.ClientCredentials,
                                               AllowOfflineAccess = true,
                                               AllowedScopes = { "openid", "profile", "api_internal_v1" }
                                           },
                                           new()
                                           {
                                               ClientId = Environment.GetEnvironmentVariable("DEVI_WEBAPP_CLIENT_ID"),
                                               ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("DEVI_WEBAPP_CLIENT_SECRET").Sha256()) },
                                               AllowedGrantTypes = GrantTypes.Code,
                                               RedirectUris = { Environment.GetEnvironmentVariable("DEVI_WEBAPP_REDIRECT_URI"), Environment.GetEnvironmentVariable("DEVI_WEBAPP_SILENT_REDIRECT_URI") },
                                               AllowOfflineAccess = true,
                                               AllowedScopes = { "openid", "profile", "api_public_v1" },
                                               AllowedCorsOrigins = { Environment.GetEnvironmentVariable("DEVI_WEBAPP_CORS_ORIGINS") },
                                               PostLogoutRedirectUris = { Environment.GetEnvironmentVariable("DEVI_WEBAPP_POST_LOGOUT_REDIRECT_URI") },
                                           }
                                       })
                   .AddAspNetIdentity<IdentityUser>()
                   .AddCustomTokenRequestValidator<CustomTokenRequestValidator>();

            builder.Services.AddAuthentication()
                            .AddDiscord(options =>
                                        {
                                            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                            options.ClientId = Environment.GetEnvironmentVariable("DEVI_DISCORD_OAUTH_CLIENT_ID")!;
                                            options.ClientSecret = Environment.GetEnvironmentVariable("DEVI_DISCORD_OAUTH_CLIENT_SECRET")!;
                                        });

            var app = builder.Build();

            app.UseCookiePolicy(new CookiePolicyOptions
                                {
                                    MinimumSameSitePolicy = SameSiteMode.None
                                });

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            var forwardOptions = new ForwardedHeadersOptions
                                 {
                                     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                                     RequireHeaderSymmetry = false,
                                 };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardOptions);

            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
               .RequireAuthorization();

            app.Run();
        }
        catch (Exception ex) when (ex is not HostAbortedException)
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }
}