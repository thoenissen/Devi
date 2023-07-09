using System;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.LookingForGroup;
using Devi.ServiceHosts.WebApi.Data.Entity;
using Devi.ServiceHosts.WebApi.Data.Entity.Repositories.LookingForGroup;
using Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Devi.ServiceHosts.WebApi.Controllers;

/// <summary>
/// Looking for group controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class LookingForGroupController : ControllerBase
{
    #region Fields

    /// <summary>
    /// Repository factory
    /// </summary>
    private readonly RepositoryFactory _repositoryFactory;

    /// <summary>
    /// Logger
    /// </summary>
    private readonly ILogger<LookingForGroupController> _logger;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repositoryFactory">Repository factory</param>
    /// <param name="logger">Logger</param>
    public LookingForGroupController(RepositoryFactory repositoryFactory,
                                     ILogger<LookingForGroupController> logger)
    {
        _repositoryFactory = repositoryFactory;
        _logger = logger;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Create appointment
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Appointments")]
    public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO dto)
    {
        if (await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                    .Add(new LookingForGroupAppointmentEntity
                                         {
                                             ChannelId = dto.ChannelId,
                                             MessageId = dto.MessageId,
                                             CreationUserId = dto.CreationUserId,
                                             Title = dto.Title,
                                             Description = dto.Description,
                                             ThreadId = dto.ThreadId
                                         })
                              .ConfigureAwait(false))
        {
            return Ok();
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Creation of appointment ({@Appointment}) failed", dto);

        return BadRequest();
    }

    /// <summary>
    /// Get appointment details
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Appointments")]
    public async Task<IActionResult> GetAppointment(ulong appointmentMessageId)
    {
        var appointmentData = await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                                      .GetQuery()
                                                      .Where(obj => obj.MessageId == appointmentMessageId)
                                                      .Select(obj => new AppointmentDetailsDTO
                                                                     {
                                                                         ChannelId = obj.ChannelId,
                                                                         Title = obj.Title,
                                                                         Description = obj.Description,
                                                                         ThreadId = obj.ThreadId,
                                                                         Participants = obj.Participants
                                                                                           .Select(obj => new ParticipantDTO
                                                                                                          {
                                                                                                              UserId = obj.UserId,
                                                                                                              RegistrationTimeStamp = obj.RegistrationTimeStamp
                                                                                                          })
                                                                                           .ToList()
                                                                     })
                                                      .FirstOrDefaultAsync()
                                                      .ConfigureAwait(false);

        if (appointmentData != null)
        {
            return Ok(appointmentData);
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Requested appointment ({AppointmentId}) not found", appointmentMessageId);

        return NotFound();
    }

    /// <summary>
    /// Refresh appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPut]
    [Route("Appointments")]
    public async Task<IActionResult> RefreshAppointment(ulong appointmentMessageId, RefreshAppointmentDTO dto)
    {
        AppointmentDTO appointmentDTO = null;

        if (await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                    .Refresh(obj => obj.MessageId == appointmentMessageId,
                                             obj =>
                                             {
                                                 if  (string.IsNullOrWhiteSpace(dto.Title) == false)
                                                 {
                                                     obj.Title = dto.Title;
                                                 }

                                                 if (string.IsNullOrWhiteSpace(dto.Description) == false)
                                                 {
                                                     obj.Description = dto.Description;
                                                 }

                                                 appointmentDTO = new AppointmentDTO
                                                                  {
                                                                      ThreadId = obj.ThreadId
                                                                  };
                                             })
                                    .ConfigureAwait(false))
        {
            return Ok(appointmentDTO);
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Refreshing appointment ({AppointmentId}) with data ({@Data}) failed", appointmentMessageId, dto);

        return BadRequest();
    }

    /// <summary>
    /// Is the given user the creator of the appointment?
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [Route("Appointments/isCreator")]
    public async Task<IActionResult> IsCreator(ulong appointmentMessageId, ulong userId)
    {
        if (await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                    .GetQuery()
                                    .AnyAsync(obj => obj.MessageId == appointmentMessageId
                                                  && obj.CreationUserId == userId)
                                    .ConfigureAwait(false))
        {
            return Ok(true);
        }

        return Ok(false);
    }

    /// <summary>
    /// Delete appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("Appointments")]
    public async Task<IActionResult> DeleteAppointment(ulong appointmentMessageId)
    {
        AppointmentDTO dto = null;

        if (await _repositoryFactory.GetRepository<LookingForGroupParticipantRepository>()
                                    .RemoveRange(obj => obj.AppointmentMessageId == appointmentMessageId)
                                    .ConfigureAwait(false)
         && await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                    .Remove(obj => obj.MessageId == appointmentMessageId,
                                            obj => dto = new AppointmentDTO
                                                         {
                                                             ThreadId = obj.ThreadId
                                                         })
                                    .ConfigureAwait(false))
        {
            return Ok(dto);
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Delete  appointment ({AppointmentId}) failed", appointmentMessageId);

        return BadRequest();
    }

    /// <summary>
    /// Add registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Registrations")]
    public async Task<IActionResult> AddRegistration(AddRegistrationDTO dto)
    {
        if (await _repositoryFactory.GetRepository<LookingForGroupParticipantRepository>()
                                    .Add(new LookingForGroupParticipantEntity
                                         {
                                             AppointmentMessageId = dto.AppointmentMessageId,
                                             RegistrationTimeStamp = DateTime.Now,
                                             UserId = dto.UserId
                                         })
                                    .ConfigureAwait(false))
        {
            var appointment = await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                                      .GetQuery()
                                                      .SelectAppointment(dto.AppointmentMessageId)
                                                      .ConfigureAwait(false);

            return Ok(appointment);
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Add registration of user ({UserId}) to  appointment ({AppointmentId}) failed", dto.UserId, dto.AppointmentMessageId);

        return BadRequest();
    }

    /// <summary>
    /// Remove registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("Registrations")]
    public async Task<IActionResult> RemoveRegistration(RemoveRegistrationDTO dto)
    {
        if (await _repositoryFactory.GetRepository<LookingForGroupParticipantRepository>()
                                    .Remove(obj => obj.AppointmentMessageId == dto.AppointmentMessageId
                                                   && obj.UserId == dto.UserId)
                                    .ConfigureAwait(false))
        {
            var appointment = await _repositoryFactory.GetRepository<LookingForGroupAppointmentRepository>()
                                                      .GetQuery()
                                                      .SelectAppointment(dto.AppointmentMessageId)
                                                      .ConfigureAwait(false);

            return Ok(appointment);
        }

        _logger.LogWarning(_repositoryFactory.LastError, "Removing registration of user ({UserId}) from appointment ({AppointmentId}) failed", dto.UserId, dto.AppointmentMessageId);

        return BadRequest();
    }

    #endregion // Methods
}