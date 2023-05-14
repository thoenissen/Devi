using System.Linq;

using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.Reminders;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Reminder;

/// <summary>
/// Queryable for accessing the <see cref="OneTimeReminderEntity"/>
/// </summary>
public class OneTimeReminderQueryable : QueryableBase<OneTimeReminderEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable"><see cref="IQueryable"/>-object</param>
    public OneTimeReminderQueryable(IQueryable<OneTimeReminderEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructor
}