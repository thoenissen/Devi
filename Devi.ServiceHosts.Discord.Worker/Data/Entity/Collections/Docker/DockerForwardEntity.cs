using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devi.ServiceHosts.Discord.Worker.Data.Entity.Collections.Docker;

/// <summary>
/// Docker Log Forwarding
/// </summary>
public class DockerForwardEntity
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
    /// Channel ID
    /// </summary>
    [BsonRepresentation(BsonType.Int64, AllowOverflow = true)]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Container name
    /// </summary>
    public string? ContainerName { get; set; }
}