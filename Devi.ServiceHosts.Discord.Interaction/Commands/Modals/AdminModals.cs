using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals;

/// <summary>
/// Admin modals
/// </summary>
[Injectable<AdminModals>(ServiceLifetime.Transient)]
public class AdminModals : LocatedInteractionModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public AdminCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Create new docker container
    /// </summary>
    /// <param name="modalData">Modal input</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [ModalInteraction("modal;admin;docker;create")]
    public Task CreateNewContainer(CreateDockerContainerModalData modalData) => CommandHandler.CreateNewContainer(Context, modalData);

    #endregion // Methods

}