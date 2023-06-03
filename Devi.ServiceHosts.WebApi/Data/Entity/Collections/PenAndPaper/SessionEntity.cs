using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;

/// <summary>
/// Session
/// </summary>
public class SessionEntity
{
    /// <summary>
    /// ID
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Campaign ID
    /// </summary>
    public ObjectId CampaignId { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Registrations
    /// </summary>
    public List<SessionRegistrationEntity> Registrations { get; set; }
}