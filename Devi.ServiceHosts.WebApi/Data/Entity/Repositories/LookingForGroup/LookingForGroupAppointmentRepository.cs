using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.LookingForGroup;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Repositories.LookingForGroup;

/// <summary>
/// Repository for accessing <see cref="LookingForGroupAppointmentEntity"/>
/// </summary>
public class LookingForGroupAppointmentRepository : RepositoryBase<LookingForGroupAppointmentQueryable, LookingForGroupAppointmentEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"><see cref="Microsoft.EntityFrameworkCore.DbContext"/>-object</param>
    public LookingForGroupAppointmentRepository(DbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructor
}