using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Devi.ServiceHosts.Clients;

/// <summary>
/// Connector base
/// </summary>
public abstract class ConnectorBase
{
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Post<T>(string route, T dto)
    {
        using (var client = _clientFactory.CreateClient())
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            using (var response = await client.PostAsync(BuildUrl(route), content)
                                              .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }

    #endregion // Public methods

    #region Private methods

    /// <summary>
    /// Build url
    /// </summary>
    /// <param name="route">Route</param>
    /// <returns>Combined url</returns>
    private Uri BuildUrl(string route) => new(_baseUrl, route);

    #endregion // Private methods
}