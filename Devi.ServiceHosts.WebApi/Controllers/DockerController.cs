using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Docker;
using Devi.ServiceHosts.WebApi.Data.Entity.Collections.Docker;
using Devi.ServiceHosts.WebApi.Services;

using Docker.DotNet;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using MongoDB.Driver;

using Serilog;

namespace Devi.ServiceHosts.WebApi.Controllers;

/// <summary>
/// Docker controller
/// </summary>
[ApiController]
[Authorize(Roles = "InternalService")]
[Route("[controller]")]
public class DockerController : ControllerBase
{
    #region Fields

    /// <summary>
    /// Mongo client factory
    /// </summary>
    private readonly MongoClientFactory _mongoFactory;

    /// <summary>
    /// Docker client factory
    /// </summary>
    private readonly DockerClientFactory _dockerFactory;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mongoFactory">Mongo client factory</param>
    /// <param name="dockerFactory">Docker client factory</param>
    public DockerController(MongoClientFactory mongoFactory,
                            DockerClientFactory dockerFactory)
    {
        _mongoFactory = mongoFactory;
        _dockerFactory = dockerFactory;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Get list of containers
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Containers")]
    public async Task<IActionResult> GetContainers(ulong serverId)
    {
        var entities = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<DockerContainerEntity>("DockerContainers")
                                          .Find(Builders<DockerContainerEntity>.Filter.Eq(obj => obj.ServerId, serverId))
                                          .SortBy(obj => obj.Name)
                                          .ToListAsync()
                                          .ConfigureAwait(false);

        var containers = entities.Select(obj => new DockerContainerDTO
                                                {
                                                    Name = obj.Name,
                                                    Description = obj.Description,
                                                })
                                 .ToList();

        var dockerClient = _dockerFactory.Create();

        foreach (var container in containers)
        {
            try
            {
                var state = await dockerClient.Containers
                                              .InspectContainerAsync(container.Name)
                                              .ConfigureAwait(false);

                if (state.State.Status == "running")
                {
                    container.IsOnline = true;
                }
            }
            catch (DockerApiException ex)
            {
                Log.Warning(ex, "Failed to inspect docker container {Name}", container.Name);
            }
        }

        return Ok(containers);
    }

    /// <summary>
    /// Create a new container
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPut]
    [Route("Containers")]
    public async Task<IActionResult> UpdateContainer(ulong serverId, [FromBody]CreateDockerContainerDTO data)
    {
        await _mongoFactory.Create()
                            .GetDatabase(_mongoFactory.Database)
                            .GetCollection<DockerContainerEntity>("DockerContainers")
                            .FindOneAndUpdateAsync(Builders<DockerContainerEntity>.Filter.Eq(obj => obj.ServerId, serverId)
                                                 & Builders<DockerContainerEntity>.Filter.Eq(obj => obj.Name, data.Name),
                                                   Builders<DockerContainerEntity>.Update
                                                                                  .SetOnInsert(obj => obj.Id, ObjectId.GenerateNewId())
                                                                                  .Set(obj => obj.ServerId, serverId)
                                                                                  .Set(obj => obj.Name, data.Name)
                                                                                  .Set(obj => obj.Description, data.Description),
                                                   new FindOneAndUpdateOptions<DockerContainerEntity> { IsUpsert= true })
                            .ConfigureAwait(false);

        return Ok();
    }

    #endregion // Methods
}