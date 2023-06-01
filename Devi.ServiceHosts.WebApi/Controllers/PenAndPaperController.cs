using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;
using Devi.ServiceHosts.WebApi.Services;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
                                               TimeStamp = firstSessionTimeStamp,
                                               Registrations = new List<SessionRegistrationEntity>()
                                           })
                           .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId,
                                                       })
                               .ConfigureAwait(false);

        return Ok();
    }

    /// <summary>
    /// Is the given user Dungeon Master of the campaign?
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Campaigns/IsDungeonMaster/{channelId}/{userId}")]
    public async Task<IActionResult> CreateContainer([FromRoute] ulong channelId, ulong userId)
    {
        if (await _mongoFactory.Create()
                               .GetDatabase(_mongoFactory.Database)
                               .GetCollection<CampaignEntity>("Campaigns")
                               .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, channelId)
                                   & Builders<CampaignEntity>.Filter.Eq(obj => obj.DungeonMasterUserId, userId))
                               .AnyAsync()
                               .ConfigureAwait(false))
        {
            return Ok();
        }

        return NotFound();
    }

    /// <summary>
    /// Get current session and campaign data
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Sessions/Current/{channelId}")]
    public async Task<IActionResult> GetCurrentSession([FromRoute] ulong channelId)
    {
        var campaign = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<CampaignEntity>("Campaigns")
                                          .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, channelId))
                                          .Project(obj => new
                                                          {
                                                              obj.Id,
                                                              obj.Name,
                                                              obj.Description,
                                                              obj.MessageId,
                                                              obj.ThreadId,
                                                              obj.DungeonMasterUserId
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);

        var session = await _mongoFactory.Create()
                                         .GetDatabase(_mongoFactory.Database)
                                         .GetCollection<SessionEntity>("Sessions")
                                         .Find(Builders<SessionEntity>.Filter.Eq(obj => obj.CampaignId, campaign.Id)
                                             & Builders<SessionEntity>.Filter.Gt(obj => obj.TimeStamp, DateTime.Now))
                                         .Project(obj => new
                                                         {
                                                             obj.TimeStamp,
                                                             obj.Registrations
                                                         })
                                         .FirstOrDefaultAsync()
                                         .ConfigureAwait(false);

        return Ok(new CurrentSessionDTO
                  {
                      Name = campaign.Name,
                      Description = campaign.Description,
                      MessageId = campaign.MessageId,
                      ThreadId = campaign.ThreadId,
                      DungeonMasterUserId = campaign.DungeonMasterUserId,
                      SessionTimeStamp = session?.TimeStamp,
                      Registrations = session?.Registrations?
                                             .Select(obj => new SessionRegistrationDTO
                                                            {
                                                                UserId = obj.UserId,
                                                                IsRegistered = obj.IsRegistered
                                                            })
                                             .ToList()
                  });
    }

    /// <summary>
    /// Join session
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Sessions/Registration")]
    public async Task<IActionResult> CreateContainer([FromBody] JoinSessionDTO data)
    {
        var campaignId = await _mongoFactory.Create()
                                            .GetDatabase(_mongoFactory.Database)
                                            .GetCollection<CampaignEntity>("Campaigns")
                                            .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId))
                                            .Project(obj => obj.Id)
                                            .FirstAsync()
                                            .ConfigureAwait(false);

        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<SessionEntity>("Sessions")
                                        .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.CampaignId, campaignId)
                                                      & Builders<SessionEntity>.Filter.Gt(obj => obj.TimeStamp, DateTime.Now)
                                                      & Builders<SessionEntity>.Filter.ElemMatch(obj => obj.Registrations, Builders<SessionRegistrationEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<SessionEntity>.Update.Set(obj => obj.Registrations.FirstMatchingElement().IsRegistered, true))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            await _mongoFactory.Create()
                               .GetDatabase(_mongoFactory.Database)
                               .GetCollection<SessionEntity>("Sessions")
                               .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.CampaignId, campaignId),
                                               Builders<SessionEntity>.Update.Push(obj => obj.Registrations,
                                                                                   new SessionRegistrationEntity
                                                                                   {
                                                                                       UserId = data.UserId,
                                                                                       IsRegistered = true
                                                                                   }))
                               .ConfigureAwait(false);
        }

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId,
                                                       })
                               .ConfigureAwait(false);

        return Ok();
    }

    /// <summary>
    /// Leave session
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("Sessions/Registration")]
    public async Task<IActionResult> CreateContainer([FromBody] LeaveSessionDTO data)
    {
        var campaignId = await _mongoFactory.Create()
                                            .GetDatabase(_mongoFactory.Database)
                                            .GetCollection<CampaignEntity>("Campaigns")
                                            .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId))
                                            .Project(obj => obj.Id)
                                            .FirstAsync()
                                            .ConfigureAwait(false);

        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<SessionEntity>("Sessions")
                                        .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.CampaignId, campaignId)
                                                      & Builders<SessionEntity>.Filter.Gt(obj => obj.TimeStamp, DateTime.Now)
                                                      & Builders<SessionEntity>.Filter.ElemMatch(obj => obj.Registrations, Builders<SessionRegistrationEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<SessionEntity>.Update.Set(obj => obj.Registrations.FirstMatchingElement().IsRegistered, false))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            await _mongoFactory.Create()
                               .GetDatabase(_mongoFactory.Database)
                               .GetCollection<SessionEntity>("Sessions")
                               .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.CampaignId, campaignId),
                                               Builders<SessionEntity>.Update.Push(obj => obj.Registrations,
                                                                                   new SessionRegistrationEntity
                                                                                   {
                                                                                       UserId = data.UserId,
                                                                                       IsRegistered = false
                                                                                   }))
                               .ConfigureAwait(false);
        }

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId,
                                                       })
                               .ConfigureAwait(false);
        return Ok();
    }

    #endregion // Methods
}