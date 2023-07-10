using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;
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
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDTO data)
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
                           DungeonMasterUserId = data.DungeonMasterUserId,
                           DayOfWeek = data.DayOfWeek,
                           Time = data.Time
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
    /// Set campaign players
    /// </summary>
    /// <param name="data">Players</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaigns/Players")]
    public async Task<IActionResult> SetPlayers([FromBody] SetPlayersDTO data)
    {
        var campaign = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<CampaignEntity>("Campaigns")
                                          .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId))
                                          .Project(obj => new
                                                          {
                                                              obj.ChannelId,
                                                              obj.ThreadId,
                                                              obj.Players
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);

        foreach (var player in campaign.Players
                                       .Where(obj => data.Players.Contains(obj.UserId) == false)
                                       .ToList())
        {
            campaign.Players.Remove(player);
        }

        foreach (var userId in data.Players
                                   .Where(obj => campaign.Players.Any(obj2 => obj2.UserId == obj) == false))
        {
            campaign.Players
                    .Add(new PlayerEntity
                         {
                             UserId = userId
                         });
        }

        await _mongoFactory.Create()
                           .GetDatabase(_mongoFactory.Database)
                           .GetCollection<CampaignEntity>("Campaigns")
                           .UpdateOneAsync(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId),
                                           Builders<CampaignEntity>.Update.Set(obj => obj.Players,
                                                                               campaign.Players))
                           .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId
                                                       })
                               .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .AddPlayers(new AddPlayersDTO
                                           {
                                               OverviewThreadId = campaign.ChannelId,
                                               LogThreadId = campaign.ThreadId,
                                               Players = data.Players,
                                           })
                               .ConfigureAwait(false);

        return Ok();
    }

    /// <summary>
    /// Add character
    /// </summary>
    /// <param name="data">Character</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaigns/Characters")]
    public async Task<IActionResult> AddCharacter([FromBody] AddCharacterDTO data)
    {
        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<CampaignEntity>("Campaigns")
                                        .UpdateOneAsync(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId)
                                                      & Builders<CampaignEntity>.Filter.ElemMatch(obj => obj.Players, Builders<PlayerEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<CampaignEntity>.Update
                                                                                .Set(obj => obj.Players.FirstMatchingElement().CharacterName, data.CharacterName)
                                                                                .Set(obj => obj.Players.FirstMatchingElement().Class, data.CharacterClass))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            return BadRequest();
        }

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId
                                                       })
                               .ConfigureAwait(false);

        return Ok();
    }

    /// <summary>
    /// Remove character
    /// </summary>
    /// <param name="data">Character</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("Campaigns/Characters")]
    public async Task<IActionResult> RemoveCharacter([FromBody] RemoveCharacterDTO data)
    {
        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<CampaignEntity>("Campaigns")
                                        .UpdateOneAsync(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId)
                                                      & Builders<CampaignEntity>.Filter.ElemMatch(obj => obj.Players, Builders<PlayerEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<CampaignEntity>.Update
                                                                                .Set(obj => obj.Players.FirstMatchingElement().CharacterName, null)
                                                                                .Set(obj => obj.Players.FirstMatchingElement().Class, null))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            return BadRequest();
        }

        await _discordConnector.PenAndPaper
                               .RefreshCampaignMessage(new RefreshCampaignMessageDTO
                                                       {
                                                           ChannelId = data.ChannelId
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
    [Route("Campaigns/{channelId}/IsDungeonMaster/{userId}")]
    public async Task<IActionResult> IsDungeonMaster([FromRoute] ulong channelId, ulong userId)
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
    /// Get overview data of the given campaign
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Campaigns/{channelId}/Overview/")]
    public async Task<IActionResult> GetCampaignOverview([FromRoute] ulong channelId)
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
                                                              obj.DungeonMasterUserId,
                                                              obj.Players
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);

        return Ok(new CampaignOverviewDTO
                  {
                      Name = campaign.Name,
                      Description = campaign.Description,
                      MessageId = campaign.MessageId,
                      ThreadId = campaign.ThreadId,
                      DungeonMasterUserId = campaign.DungeonMasterUserId,
                      Players = campaign.Players
                                        ?.Select(obj => new PlayerDTO
                                                        {
                                                            UserId = obj.UserId,
                                                            CharacterName = obj.CharacterName,
                                                            Class = obj.Class
                                                        })
                                        .ToList()
                  });
    }

    /// <summary>
    /// Create session
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Sessions")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionDTO data)
    {
        var campaign = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<CampaignEntity>("Campaigns")
                                          .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.ChannelId, data.ChannelId))
                                          .Project(obj => new
                                                          {
                                                              obj.Id,
                                                              obj.Players,
                                                              obj.ThreadId
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);
        await _mongoFactory.Create()
                           .GetDatabase(_mongoFactory.Database)
                           .GetCollection<SessionEntity>("Sessions")
                           .InsertOneAsync(new SessionEntity
                                           {
                                               Id = ObjectId.GenerateNewId(),
                                               CampaignId = campaign.Id,
                                               MessageId = data.MessageId,
                                               TimeStamp = data.TimeStamp,
                                               Registrations = campaign.Players
                                                                       .Select(obj => new SessionRegistrationEntity
                                                                                      {
                                                                                          UserId = obj.UserId,
                                                                                          IsRegistered = true
                                                                                      })
                                                                       .ToList()
                                           })
                           .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .RefreshSessionMessage(new RefreshSessionMessageDTO
                                                      {
                                                          MessageId = data.MessageId
                                                      })
                               .ConfigureAwait(false);

        await _discordConnector.PenAndPaper.PostLogMessage(new PostLogMessageDTO<SessionCreatedDTO>
                                                           {
                                                               Type = LogMessageType.SessionCreated,
                                                               Content = new SessionCreatedDTO
                                                                         {
                                                                             ChannelId = data.ChannelId,
                                                                             MessageId = data.MessageId,
                                                                             TimeStamp = data.TimeStamp
                                                                         }
                                                           },
                                                           campaign.ThreadId)
                               .ConfigureAwait(false);

        return Ok();
    }

    /// <summary>
    /// Get session
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Sessions/{messageId}")]
    public async Task<IActionResult> GetSession([FromRoute] ulong messageId)
    {
        var database = _mongoFactory.Create()
                                    .GetDatabase(_mongoFactory.Database);

        var campaignsCollection = database.GetCollection<CampaignEntity>("Campaigns");

        var session = await database.GetCollection<SessionEntity>("Sessions")
                                    .Aggregate()
                                    .Match(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, messageId))
                                    .Lookup<SessionEntity, CampaignEntity, SessionWithCampaignEntity>(campaignsCollection,
                                            obj => obj.CampaignId,
                                            obj => obj.Id,
                                            obj => obj.Campaign)
                                    .Unwind<SessionWithCampaignEntity, SessionWithCampaignEntity>(obj => obj.Campaign)
                                    .Project(obj => new
                                                    {
                                                        obj.Campaign.ChannelId,
                                                        obj.TimeStamp,
                                                        obj.Registrations,
                                                    })
                                    .FirstAsync()
                                    .ConfigureAwait(false);

        return Ok(new SessionDTO
                  {
                      ChannelId = session.ChannelId,
                      TimeStamp = session.TimeStamp,
                      Registrations = session.Registrations
                                             .Select(obj => new SessionRegistrationDTO
                                                            {
                                                                UserId = obj.UserId,
                                                                IsRegistered = obj.IsRegistered,
                                                            })
                                             .ToList()
                  });
    }

    /// <summary>
    /// Delete session
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("Sessions/{messageId}")]
    public async Task<IActionResult> DeleteSession([FromRoute] ulong messageId)
    {
        var session = await _mongoFactory.Create()
                                         .GetDatabase(_mongoFactory.Database)
                                         .GetCollection<SessionEntity>("Sessions")
                                         .FindOneAndDeleteAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, messageId))
                                         .ConfigureAwait(false);

        if (session.TimeStamp > DateTime.Now)
        {
            var campaign = await _mongoFactory.Create()
                                              .GetDatabase(_mongoFactory.Database)
                                              .GetCollection<CampaignEntity>("Campaigns")
                                              .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.Id, session.CampaignId))
                                              .Project(obj => new
                                                              {
                                                                  obj.ThreadId
                                                              })
                                              .FirstAsync()
                                              .ConfigureAwait(false);

            await _discordConnector.PenAndPaper.PostLogMessage(new PostLogMessageDTO<SessionDeletedDTO>
                                                               {
                                                                   Type = LogMessageType.SessionDeleted,
                                                                   Content = new SessionDeletedDTO
                                                                             {
                                                                                 TimeStamp = session.TimeStamp
                                                                             }
                                                               },
                                                               campaign.ThreadId)
                                   .ConfigureAwait(false);
        }

        return Ok();
    }

    /// <summary>
    /// Join session
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Sessions/Registration")]
    public async Task<IActionResult> AddRegistration([FromBody] JoinSessionDTO data)
    {
        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<SessionEntity>("Sessions")
                                        .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId)
                                                      & Builders<SessionEntity>.Filter.ElemMatch(obj => obj.Registrations, Builders<SessionRegistrationEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<SessionEntity>.Update.Set(obj => obj.Registrations.FirstMatchingElement().IsRegistered, true))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            await _mongoFactory.Create()
                               .GetDatabase(_mongoFactory.Database)
                               .GetCollection<SessionEntity>("Sessions")
                               .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId),
                                               Builders<SessionEntity>.Update.Push(obj => obj.Registrations,
                                                                                   new SessionRegistrationEntity
                                                                                   {
                                                                                       UserId = data.UserId,
                                                                                       IsRegistered = true
                                                                                   }))
                               .ConfigureAwait(false);
        }

        await _discordConnector.PenAndPaper
                               .RefreshSessionMessage(new RefreshSessionMessageDTO
                                                       {
                                                           MessageId = data.MessageId,
                                                       })
                               .ConfigureAwait(false);

        var session = await _mongoFactory.Create()
                                         .GetDatabase(_mongoFactory.Database)
                                         .GetCollection<SessionEntity>("Sessions")
                                         .Find(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId))
                                         .Project(obj => new
                                                         {
                                                             obj.CampaignId,
                                                             obj.TimeStamp
                                                         })
                                         .FirstAsync()
                                         .ConfigureAwait(false);

        var campaign = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<CampaignEntity>("Campaigns")
                                          .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.Id, session.CampaignId))
                                          .Project(obj => new
                                                          {
                                                              obj.ThreadId
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .PostLogMessage(new PostLogMessageDTO<UserJoinedDTO>
                                               {
                                                   Type = LogMessageType.UserJoined,
                                                   Content = new UserJoinedDTO
                                                             {
                                                                 UserId = data.UserId,
                                                                 SessionTimeStamp = session.TimeStamp
                                                             }
                                               },
                                               campaign.ThreadId)
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
    public async Task<IActionResult> RemoveRegistration([FromBody] LeaveSessionDTO data)
    {
        var result = await _mongoFactory.Create()
                                        .GetDatabase(_mongoFactory.Database)
                                        .GetCollection<SessionEntity>("Sessions")
                                        .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId)
                                                      & Builders<SessionEntity>.Filter.ElemMatch(obj => obj.Registrations, Builders<SessionRegistrationEntity>.Filter.Eq(obj => obj.UserId, data.UserId)),
                                                        Builders<SessionEntity>.Update.Set(obj => obj.Registrations.FirstMatchingElement().IsRegistered, false))
                                        .ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            await _mongoFactory.Create()
                               .GetDatabase(_mongoFactory.Database)
                               .GetCollection<SessionEntity>("Sessions")
                               .UpdateOneAsync(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId),
                                               Builders<SessionEntity>.Update.Push(obj => obj.Registrations,
                                                                                   new SessionRegistrationEntity
                                                                                   {
                                                                                       UserId = data.UserId,
                                                                                       IsRegistered = false
                                                                                   }))
                               .ConfigureAwait(false);
        }

        await _discordConnector.PenAndPaper
                               .RefreshSessionMessage(new RefreshSessionMessageDTO
                                                       {
                                                           MessageId = data.MessageId,
                                                       })
                               .ConfigureAwait(false);

        var session = await _mongoFactory.Create()
                                         .GetDatabase(_mongoFactory.Database)
                                         .GetCollection<SessionEntity>("Sessions")
                                         .Find(Builders<SessionEntity>.Filter.Eq(obj => obj.MessageId, data.MessageId))
                                         .Project(obj => new
                                                         {
                                                             obj.CampaignId,
                                                             obj.TimeStamp
                                                         })
                                         .FirstAsync()
                                         .ConfigureAwait(false);

        var campaign = await _mongoFactory.Create()
                                          .GetDatabase(_mongoFactory.Database)
                                          .GetCollection<CampaignEntity>("Campaigns")
                                          .Find(Builders<CampaignEntity>.Filter.Eq(obj => obj.Id, session.CampaignId))
                                          .Project(obj => new
                                                          {
                                                              obj.ThreadId
                                                          })
                                          .FirstAsync()
                                          .ConfigureAwait(false);

        await _discordConnector.PenAndPaper
                               .PostLogMessage(new PostLogMessageDTO<UserLeftDTO>
                                               {
                                                   Type = LogMessageType.UserLeft,
                                                   Content = new UserLeftDTO
                                                             {
                                                                 UserId = data.UserId,
                                                                 SessionTimeStamp = session.TimeStamp,
                                                             }
                                               },
                                               campaign.ThreadId)
                               .ConfigureAwait(false);

        return Ok();
    }

    #endregion // Methods
}