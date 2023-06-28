using System.Linq;

using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Queryable.LookingForGroup;

/// <summary>
/// Queryable for accessing the <see cref="LookingForGroupParticipantEntity"/>
/// </summary>
public class LookingForGroupParticipantQueryable : QueryableBase<LookingForGroupParticipantEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable"><see cref="IQueryable"/>-object</param>
    public LookingForGroupParticipantQueryable(IQueryable<LookingForGroupParticipantEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructor
}