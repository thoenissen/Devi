namespace Devi.TestConsole;

/// <summary>
/// Client factory
/// </summary>
internal class HttpClientFactory : IHttpClientFactory
{
    #region IHttpClientFactory

    /// <summary>
    /// Creates and configures an <see cref="T:System.Net.Http.HttpClient" /> instance using the configuration that corresponds
    /// to the logical name specified by <paramref name="name" />.
    /// </summary>
    /// <param name="name">The logical name of the client to create.</param>
    /// <returns>A new <see cref="T:System.Net.Http.HttpClient" /> instance.</returns>
    /// <remarks>
    /// <para>
    /// Each call to <see cref="M:System.Net.Http.IHttpClientFactory.CreateClient(System.String)" /> is guaranteed to return a new <see cref="T:System.Net.Http.HttpClient" />
    /// instance. It is generally not necessary to dispose of the <see cref="T:System.Net.Http.HttpClient" /> as the
    /// <see cref="T:System.Net.Http.IHttpClientFactory" /> tracks and disposes resources used by the <see cref="T:System.Net.Http.HttpClient" />.
    /// </para>
    /// <para>
    /// Callers are also free to mutate the returned <see cref="T:System.Net.Http.HttpClient" /> instance's public properties
    /// as desired.
    /// </para>
    /// </remarks>
    public HttpClient CreateClient(string name)
    {
        return new HttpClient();
    }

    #endregion // IHttpClientFactory
}