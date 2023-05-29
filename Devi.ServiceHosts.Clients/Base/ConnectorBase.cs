using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

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
    private class Void
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

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientFactory">Client factory</param>
    /// <param name="baseUrl">Base url</param>
    protected ConnectorBase(IHttpClientFactory clientFactory, string baseUrl)
    {
        _baseUrl = new Uri(baseUrl);
        _clientFactory = clientFactory;
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
    public Task Post<T>(string route, T dto, NameValueCollection parameters = null) => Send<T, Void>(HttpMethod.Post, route, dto, parameters);

    /// <summary>
    /// Put
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="dto">DTO</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Put<T>(string route, T dto, NameValueCollection parameters = null) => Send<T, Void>(HttpMethod.Put, route, dto, parameters);

    /// <summary>
    /// Get
    /// </summary>
    /// <typeparam name="T">DTO type</typeparam>
    /// <param name="route">Route</param>
    /// <param name="parameters">parameters</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<T> Get<T>(string route, NameValueCollection parameters = null) => Send<Void, T>(HttpMethod.Get, route, null, parameters);

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

    #endregion // Private methods
}