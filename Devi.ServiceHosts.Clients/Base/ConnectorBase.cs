using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IdentityModel.Client;

using Newtonsoft.Json;

using Serilog;

namespace Devi.ServiceHosts.Clients.Base;

/// <summary>
/// Connector base
/// </summary>
public abstract class ConnectorBase
{
    #region Nested classes

    /// <summary>
    /// Dummy type
    /// </summary>
    protected class Void
    {
    }

    #endregion // Nested classes

    #region Fields

    /// <summary>
    /// Client factory
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Base url
    /// </summary>
    private readonly Uri _baseUrl;

    /// <summary>
    /// Should a access token been used?
    /// </summary>
    private bool _isUseAccessToken;

    /// <summary>
    /// Access token endpoint
    /// </summary>
    private string _accessTokenEndPoint;

    /// <summary>
    /// Identity server address
    /// </summary>
    private string _identityServerAddress;

    /// <summary>
    /// Client ID
    /// </summary>
    private string _clientId;

    /// <summary>
    /// Client secret
    /// </summary>
    private string _clientSecret;

    /// <summary>
    /// Current access token
    /// </summary>
    private TokenResponse _accessToken;

    /// <summary>
    /// Token expiration
    /// </summary>
    private DateTime? _expirationTimeStamp;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientFactory">Client factory</param>
    /// <param name="baseUrl">Base url</param>
    /// <param name="isUseAccessToken">Should a access token been used?</param>
    protected ConnectorBase(IHttpClientFactory clientFactory, string baseUrl, bool isUseAccessToken)
    {
        _baseUrl = new Uri(baseUrl);
        _clientFactory = clientFactory;

        if (isUseAccessToken)
        {
            _clientId = Environment.GetEnvironmentVariable("DEVI_WEBAPI_CLIENT_ID");
            _clientSecret = Environment.GetEnvironmentVariable("DEVI_WEBAPI_CLIENT_SECRET");
            _identityServerAddress = Environment.GetEnvironmentVariable("DEVI_IDENTITY_SERVER_URL");

            if (string.IsNullOrEmpty(_clientId)
             || string.IsNullOrEmpty(_clientSecret)
             || string.IsNullOrEmpty(_identityServerAddress))
            {
                throw new InvalidOperationException("Invalid authentication configuration.");
            }

            _isUseAccessToken = true;
        }
    }

    #endregion // Constructor

    #region Public methods

    /// <summary>
    /// Post
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task Post<T>(string route, T dto, NameValueCollection parameters = null) => Send<T, Void>(HttpMethod.Post, route, dto, parameters);

    /// <summary>
    /// Post
    /// </summary>
    /// <typeparam name="TIn">Input DTO type</typeparam>
    /// <typeparam name="TOut">Output DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task<TOut> Post<TIn, TOut>(string route, TIn dto, NameValueCollection parameters = null) => Send<TIn, TOut>(HttpMethod.Post, route, dto, parameters);

    /// <summary>
    /// Put
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task Put<T>(string route, T dto, NameValueCollection parameters = null) => Send<T, Void>(HttpMethod.Put, route, dto, parameters);

    /// <summary>
    /// Put
    /// </summary>
    /// <typeparam name="TIn">Input DTO type</typeparam>
    /// <typeparam name="TOut">Output DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task<TOut> Put<TIn, TOut>(string route, TIn dto, NameValueCollection parameters = null) => Send<TIn, TOut>(HttpMethod.Put, route, dto, parameters);

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="route">Route</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task Get(string route, NameValueCollection parameters = null) => Send<Void, Void>(HttpMethod.Get, route, null, parameters);

    /// <summary>
    /// Get
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task<T> Get<T>(string route, NameValueCollection parameters = null) => Send<Void, T>(HttpMethod.Get, route, null, parameters);

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="route">Route</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task Delete(string route, NameValueCollection parameters = null) => Send<Void, Void>(HttpMethod.Delete, route, null, parameters);

    /// <summary>
    /// Delete
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task Delete<T>(string route, T dto, NameValueCollection parameters = null) => Send<T, Void>(HttpMethod.Delete, route, dto, parameters);

    /// <summary>
    /// Delete
    /// </summary>
    /// <typeparam name="TIn">Input DTO type</typeparam>
    /// <typeparam name="TOut">Output DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task<TOut> Delete<TIn, TOut>(string route, TIn dto, NameValueCollection parameters = null) => Send<TIn, TOut>(HttpMethod.Delete, route, dto, parameters);

    #endregion // Public methods

    #region Private methods

    /// <summary>
    /// Get
    /// </summary>
    /// <typeparam name="TIn">Input DTO type</typeparam>
    /// <typeparam name="TOut">Output DTO type</typeparam>
    /// <param name="method">Methods</param>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task<TOut> Send<TIn, TOut>(HttpMethod method, string route, TIn dto, NameValueCollection parameters = null)
    {
        try
        {
            await RefreshAccessToken(false).ConfigureAwait(false);

            return await SendInternal<TIn, TOut>(method, route, dto, parameters).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await RefreshAccessToken(false).ConfigureAwait(false);

            return await SendInternal<TIn, TOut>(method, route, dto, parameters).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <typeparam name="TIn">Input DTO type</typeparam>
    /// <typeparam name="TOut">Output DTO type</typeparam>
    /// <param name="method">Methods</param>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task<TOut> SendInternal<TIn, TOut>(HttpMethod method, string route, TIn dto, NameValueCollection parameters = null)
    {
        using (var client = _clientFactory.CreateClient())
        {
            HttpContent content = null;

            if (dto != null)
            {
                content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            }

            var request = new HttpRequestMessage(method, BuildUri(route, parameters))
                          {
                              Content = content
                          };

            request.Content = content;

            if (_accessToken?.AccessToken != null)
            {
                request.SetBearerToken(_accessToken.AccessToken);
            }

            using (var response = await client.SendAsync(request)
                                              .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                if (typeof(TOut) != typeof(Void))
                {
                    var jsonResult = await response.Content
                                                   .ReadAsStringAsync()
                                                   .ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<TOut>(jsonResult);
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Build url
    /// </summary>
    /// <param name="route">Route</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Combined url</returns>
    private Uri BuildUri(string route, NameValueCollection parameters)
    {
        var uri = new Uri(_baseUrl, route);

        if (parameters?.Count > 0)
        {
            var sb = new StringBuilder();

            foreach (string key in parameters.Keys)
            {
                var values = parameters.GetValues(key);

                if (values != null)
                {
                    foreach (var value in values)
                    {
                        sb.Append(sb.Length == 0 ? "?" : "&");
                        sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                    }
                }
            }

            uri = new Uri(uri, sb.ToString());
        }

        return uri;
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="forceRefresh">Force creation of a new access token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task RefreshAccessToken(bool forceRefresh)
    {
        if (_isUseAccessToken)
        {
            using (var client = _clientFactory.CreateClient())
            {
                if (string.IsNullOrWhiteSpace(_accessTokenEndPoint))
                {
                    var discoveryDocumentRequest = new DiscoveryDocumentRequest
                                                   {
                                                       Address = _identityServerAddress,
                                                       Policy = new DiscoveryPolicy
                                                                {
                                                                    LoopbackAddresses = new HashSet<string>
                                                                                        {
                                                                                            "localhost",
                                                                                            "127.0.0.1",
                                                                                            "host.docker.internal",
                                                                                            "devi.servicehosts.identityserver"
                                                                                        }
                                                                }
                                                   };

                    var discoveryDocument = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest)
                                                        .ConfigureAwait(false);
                    if (discoveryDocument.IsError == false)
                    {
                        _accessTokenEndPoint = discoveryDocument.TokenEndpoint;

                        Log.Information("Using the following token endpoint: {Endpoint}", discoveryDocument.TokenEndpoint);
                    }
                    else
                    {
                        Log.Error("Determination of access token endpoint failed. ({Description})", discoveryDocument.Error);
                    }
                }

                if (string.IsNullOrWhiteSpace(_accessTokenEndPoint) == false)
                {
                    if (forceRefresh
                     || _accessToken?.RefreshToken == null)
                    {
                        _accessToken = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                                                                                       {
                                                                                           Address = _accessTokenEndPoint,
                                                                                           ClientId = _clientId,
                                                                                           ClientSecret = _clientSecret,
                                                                                           Scope = "api_internal_v1"
                                                                                       })
                                                   .ConfigureAwait(false);

                        if (_accessToken.IsError == false)
                        {
                            Log.Error("Requesting new access token failed. ({Description})", _accessToken.ErrorDescription);

                            _expirationTimeStamp = DateTime.Now
                                                           .AddSeconds(_accessToken.ExpiresIn)
                                                           .AddMinutes(-15);
                        }
                        else
                        {
                            Log.Information("New access token requested.");

                            _accessToken = null;
                        }
                    }
                    else if (_expirationTimeStamp < DateTime.Now)
                    {
                        _accessToken = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                                                                             {
                                                                                 Address = _accessTokenEndPoint,
                                                                                 ClientId = _clientId,
                                                                                 RefreshToken = _accessToken.RefreshToken
                                                                             })
                                                   .ConfigureAwait(false);

                        if (_accessToken.IsError)
                        {
                            _accessToken = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                                                                                           {
                                                                                               Address = _accessTokenEndPoint,
                                                                                               ClientId = _clientId,
                                                                                               ClientSecret = _clientSecret,
                                                                                               Scope = "api_internal_v1"
                                                                                           })
                                                       .ConfigureAwait(false);
                        }

                        if (_accessToken.IsError == false)
                        {
                            _expirationTimeStamp = DateTime.Now
                                                           .AddSeconds(_accessToken.ExpiresIn)
                                                           .AddMinutes(-15);
                        }
                        else
                        {
                            _accessToken = null;
                        }
                    }
                    else
                    {
                        Log.Debug("Using existing access token.");
                    }
                }
                else
                {
                    Log.Error("Access token endpoint not available.");
                }
            }
        }
    }

    #endregion // Private methods
}