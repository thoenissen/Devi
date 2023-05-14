using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Reminders;
using Devi.ServiceHosts.WebApi.Data.Entity;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Reminder;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.Reminders;
using Devi.ServiceHosts.WebApi.Jobs.Reminders;
using Devi.ServiceHosts.WebApi.Services;

using Microsoft.AspNetCore.Mvc;

namespace Devi.ServiceHosts.WebApi.Controllers;

/// <summary>
/// Reminder controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class RemindersController : ControllerBase
{
    #region Fields

    /// <summary>
    /// Repository factory
    /// </summary>
    private readonly RepositoryFactory _repositoryFactory;

    /// <summary>
    /// Job scheduler
    /// </summary>
    private readonly JobScheduler _jobScheduler;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repositoryFactory">Repository factory</param>
    /// <param name="jobScheduler">Job scheduler</param>
    public RemindersController(RepositoryFactory repositoryFactory,
                               JobScheduler jobScheduler)
    {
        _repositoryFactory = repositoryFactory;
        _jobScheduler = jobScheduler;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOneTimeReminder([FromBody] CreateOneTimeReminderDTO data)
    {
        var entity = new OneTimeReminderEntity
                     {
                         DiscordUserId = data.UserId,
                         DiscordChannelId = data.ChannelId,
                         Message = data.Message,
                         TimeStamp = data.TimeStamp
                     };

        if (await _repositoryFactory.GetRepository<OneTimeReminderRepository>()
                                    .Add(entity)
                                    .ConfigureAwait(false))
        {
            _jobScheduler.AddJob(new OneTimeReminderJob(entity.Id), entity.TimeStamp);

            return Ok();
        }

        return BadRequest();
    }

    #endregion // Methods
}