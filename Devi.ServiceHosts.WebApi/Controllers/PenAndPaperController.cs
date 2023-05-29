﻿using System;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;
using Devi.ServiceHosts.WebApi.Services;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

namespace Devi.ServiceHosts.WebApi.Controllers;

/// <summary>
/// Pen and paper controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class PenAndPaperController : ControllerBase
{
    #region Fields

    /// <summary>
    /// Mongo client factory
    /// </summary>
    private readonly MongoClientFactory _mongoFactory;

    /// <summary>
    /// Discord connector
    /// </summary>
    private readonly DiscordConnector _discordConnector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mongoFactory">Mongo client factory</param>
    /// <param name="discordConnector">Discord connector</param>
    public PenAndPaperController(MongoClientFactory mongoFactory,
                                 DiscordConnector discordConnector)
    {
        _mongoFactory = mongoFactory;
        _discordConnector = discordConnector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Create a new campaign
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaigns")]
    public async Task<IActionResult> CreateContainer([FromBody] CreateCampaignDTO data)
    {
        var firstSessionTimeStamp = DateTime.Today.Add(data.Time);

        while (firstSessionTimeStamp.DayOfWeek != data.DayOfWeek
            || firstSessionTimeStamp < DateTime.Now)
        {
            firstSessionTimeStamp = firstSessionTimeStamp.AddDays(1);
        }

        var campaign = new CampaignEntity
                       {
                           Id = ObjectId.GenerateNewId(),
                           Name = data.Name,
                           Description = data.Description.TrimEnd(),
                           ChannelId = data.ChannelId,
                           MessageId = data.MessageId,
                           ThreadId = data.ThreadId,
                           DungeonMasterUserId = data.DungeonMasterUserId
                       };

        await _mongoFactory.Create()
                           .GetDatabase(_mongoFactory.Database)
                           .GetCollection<CampaignEntity>("Campaigns")
                           .InsertOneAsync(campaign)
                           .ConfigureAwait(false);

        await _mongoFactory.Create()
                           .GetDatabase(_mongoFactory.Database)
                           .GetCollection<SessionEntity>("Sessions")
                           .InsertOneAsync(new SessionEntity
                                           {
                                               Id = ObjectId.GenerateNewId(),
                                               CampaignId = campaign.Id,
                                               TimeStamp = firstSessionTimeStamp
                                           })
                           .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           Name = data.Name,
                                                           Description = data.Description,
                                                           ChannelId = data.ChannelId,
                                                           MessageId = data.MessageId,
                                                           ThreadId = data.ThreadId,
                                                           DungeonMasterUserId = data.DungeonMasterUserId,
                                                           SessionTimeStamp = firstSessionTimeStamp
                                                       })
                               .ConfigureAwait(false);

        return Ok();
    }

    #endregion // Methods
}