using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.LookingForGroup;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Repositories.LookingForGroup;

/// <summary>
/// Repository for accessing <see cref="LookingForGroupParticipantEntity"/>
/// </summary>
public class LookingForGroupParticipantRepository : RepositoryBase<LookingForGroupParticipantQueryable, LookingForGroupParticipantEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"><see cref="Microsoft.EntityFrameworkCore.DbContext"/>-object</param>
    public LookingForGroupParticipantRepository(DbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructor
}