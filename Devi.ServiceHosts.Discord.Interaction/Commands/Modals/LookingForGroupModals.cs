using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals;

/// <summary>
/// Guild modals
/// </summary>
[Injectable<LookingForGroupModals>(ServiceLifetime.Transient)]
public class LookingForGroupModals : LocatedInteractionModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public LookingForGroupCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Creation
    /// </summary>
    /// <param name="modal">Modal input</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [ModalInteraction(LookingForGroupCreationModalData.CustomId)]
    public Task Creation(LookingForGroupCreationModalData modal) => CommandHandler.Create(Context, modal.AppointmentTitle, modal.AppointmentDescription);

    /// <summary>
    /// Edit
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="modal">Modal input</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [ModalInteraction($"{LookingForGroupEditModalData.CustomId};*")]
    public Task Edit(ulong appointmentMessageId, LookingForGroupEditModalData modal) => CommandHandler.Edit(Context, appointmentMessageId, modal.AppointmentTitle, modal.AppointmentDescription);

    #endregion // Methods
}