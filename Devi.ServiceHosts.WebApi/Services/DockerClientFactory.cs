using System;
using Devi.ServiceHosts.Core.ServiceProvider;

using Docker.DotNet;

namespace Devi.ServiceHosts.WebApi.Services;

/// <summary>
/// Mongo client factory
/// </summary>
public sealed class DockerClientFactory : LocatedSingletonServiceBase
{
    #region Fields

    /// <summary>
    /// Connection
    /// </summary>
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("DEVI_DOCKER_CONNECTION");

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Starting the job server
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public DockerClient Create() => new DockerClientConfiguration(new Uri(_connectionString)).CreateClient();

    #endregion // Methods
}