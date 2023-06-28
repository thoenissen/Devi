using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Discord;
using Devi.ServiceHosts.DTOs.Reminders;
using Devi.ServiceHosts.WebApi.Data.Entity;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Reminder;
using Devi.ServiceHosts.WebApi.Jobs.Base;

namespace Devi.ServiceHosts.WebApi.Jobs.Reminders;

/// <summary>
/// Execution of a one time reminder
/// </summary>
public class OneTimeReminderJob : LocatedAsyncJob
{
    #region Fields

    /// <summary>
    /// Id of the reminder
    /// </summary>
    private long _id;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">Entity id</param>
    public OneTimeReminderJob(long id)
    {
        _id = id;
    }

    #endregion // Constructor

    #region IJob

    /// <summary>
    /// Executes the job.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task ExecuteOverrideAsync()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var transaction = dbFactory.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
            await using (transaction.ConfigureAwait(false))
            {
                var jobEntity = dbFactory.GetRepository<OneTimeReminderRepository>()
                                         .GetQuery()
                                         .Where(obj => obj.Id == _id)
                                         .Select(obj => new
                                                        {
                                                            ChannelId = obj.DiscordChannelId,
                                                            obj.IsExecuted,
                                                            obj.DiscordUserId,
                                                            obj.Message
                                                        })
                                         .FirstOrDefault();

                if (jobEntity?.IsExecuted == false)
                {
                    if (await dbFactory.GetRepository<OneTimeReminderRepository>()
                                       .Refresh(obj => obj.Id == _id,
                                                obj => obj.IsExecuted = true)
                                       .ConfigureAwait(false))
                    {
                        await transaction.CommitAsync()
                                         .ConfigureAwait(false);

                        var isExecuted = false;

                        try
                        {
                            await GetService<DiscordConnector>().Reminders
                                                                .PostOneTimeReminder(new PostReminderMessageDTO
                                                                                     {
                                                                                         UserId = jobEntity.DiscordUserId,
                                                                                         ChannelId = jobEntity.ChannelId,
                                                                                         Message = jobEntity.Message
                                                                                     })
                                                                .ConfigureAwait(false);

                            isExecuted = true;
                        }
                        finally
                        {
                            if (isExecuted == false)
                            {
                                await dbFactory.GetRepository<OneTimeReminderRepository>()
                                               .Refresh(obj => obj.Id == _id,
                                                        obj => obj.IsExecuted = false)
                                               .ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion // IJob
}