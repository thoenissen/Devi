using System;

using Devi.Core.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

namespace Devi.ServiceHosts.WebApi.Services;

/// <summary>
/// Mongo client factory
/// </summary>
[Injectable<MongoClientFactory>(ServiceLifetime.Singleton)]
public sealed class MongoClientFactory
{
    #region Fields

    /// <summary>
    /// Connection
    /// </summary>
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("DEVI_MONGODB_CONNECTION");

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Database
    /// </summary>
    public string Database { get; } = Environment.GetEnvironmentVariable("DEVI_MONGODB_DATABASE");

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Starting the job server
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public MongoClient Create() => new(_connectionString);

    #endregion // Methods
}