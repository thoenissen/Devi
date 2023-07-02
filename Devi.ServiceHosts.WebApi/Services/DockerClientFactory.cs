using System;
using Devi.Core.DependencyInjection;

using Docker.DotNet;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.WebApi.Services;

/// <summary>
/// Mongo client factory
/// </summary>
[Injectable<DockerClientFactory>(ServiceLifetime.Singleton)]
public sealed class DockerClientFactory
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