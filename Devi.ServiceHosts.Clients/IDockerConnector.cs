using System.Collections.Generic;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Docker;

namespace Devi.ServiceHosts.Clients;

/// <summary>
/// Docker connector
/// </summary>
public interface IDockerConnector
{
    /// <summary>
    /// Get docker containers
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<List<DockerContainerDTO>> GetDockerContainers(ulong serverId);

    /// <summary>
    /// Add or refresh container
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <param name="container">Container</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddOrRefreshContainer(ulong serverId, DockerContainerDTO container);
}