using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Devi.ServiceHosts.IdentityServer.Data;

/// <summary>
/// Identity server database context
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    #region Fields

    /// <summary>
    /// Connection string
    /// </summary>
    private static string _connectionString;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #endregion // Constructor

    #region DbContext

    /// <summary>
    /// <para>
    /// Override this method to configure the database (and other options) to be used for this context.
    /// This method is called for each instance of the context that is created.
    /// The base implementation does nothing.
    /// </para>
    /// <para>
    /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions"/> may or may not have been passed
    /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured"/> to determine if
    /// the options have already been set, and skip some or all of the logic in
    /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)"/>.
    /// </para>
    /// </summary>
    /// <param name="optionsBuilder">
    /// A builder used to create or modify options for this context. Databases (and other extensions)
    /// typically define extension methods on this object that allow you to configure the context.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString == null)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
                                          {
                                              ApplicationName = "Devi.ServiceHosts.WebApi",
                                              DataSource = Environment.GetEnvironmentVariable("DEVI_DB_DATA_SOURCE"),
                                              InitialCatalog = Environment.GetEnvironmentVariable("DEVI_DB_CATALOG"),
                                              MultipleActiveResultSets = false,
                                              IntegratedSecurity = false,
                                              UserID = Environment.GetEnvironmentVariable("DEVI_DB_USER"),
                                              Password = Environment.GetEnvironmentVariable("DEVI_DB_PASSWORD"),
                                              TrustServerCertificate = true
                                          };

            _connectionString = connectionStringBuilder.ConnectionString;
        }

        optionsBuilder.UseSqlServer(_connectionString);
#if DEBUG
        optionsBuilder.LogTo(s => System.Diagnostics.Debug.WriteLine(s));
#endif
        base.OnConfiguring(optionsBuilder);
    }

    #endregion // DbContext
}