using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.LookingForGroup;
using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Base;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

using Microsoft.EntityFrameworkCore;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Queryable.LookingForGroup;

/// <summary>
/// Queryable for accessing the <see cref="LookingForGroupAppointmentEntity"/>
/// </summary>
public class LookingForGroupAppointmentQueryable : QueryableBase<LookingForGroupAppointmentEntity>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable"><see cref="IQueryable"/>-object</param>
    public LookingForGroupAppointmentQueryable(IQueryable<LookingForGroupAppointmentEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Select appointment base data
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<AppointmentDTO> SelectAppointment(ulong appointmentMessageId)
    {
        return InternalQueryable.Where(obj => obj.MessageId == appointmentMessageId)
                                .Select(obj => new AppointmentDTO
                                               {
                                                   ThreadId = obj.ThreadId,
                                               })
                                .FirstOrDefaultAsync();
    }

    #endregion // Methods
}