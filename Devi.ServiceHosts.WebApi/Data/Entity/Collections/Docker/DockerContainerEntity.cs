using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.Docker;

/// <summary>
/// Docker container
/// </summary>
public class DockerContainerEntity
{
    /// <summary>
    /// ID
    /// </summary>
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Server ID
    /// </summary>
    [BsonRepresentation(BsonType.Int64, AllowOverflow = true)]
    public ulong ServerId { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }
}