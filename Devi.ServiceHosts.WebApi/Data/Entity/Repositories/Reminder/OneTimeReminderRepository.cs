using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Reminder;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.Reminders;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Reminder;

/// <summary>
/// Repository for accessing <see cref="OneTimeReminderEntity"/>
/// </summary>
public class OneTimeReminderRepository : RepositoryBase<OneTimeReminderQueryable, OneTimeReminderEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"><see cref="DbContext"/>-object</param>
    public OneTimeReminderRepository(DbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructor
}