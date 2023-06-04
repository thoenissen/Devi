using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;

/// <summary>
/// Campaign
/// </summary>
public class CampaignEntity
{
    /// <summary>
    /// ID
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Thread ID
    /// </summary>
    public ulong ThreadId { get; set; }

    /// <summary>
    /// Dungeon master user ID
    /// </summary>
    public ulong DungeonMasterUserId { get; set; }

    /// <summary>
    /// Players
    /// </summary>
    public List<PlayerEntity> Players { get; set; }

    /// <summary>
    /// Day of week
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    public TimeSpan Time { get; set; }
}