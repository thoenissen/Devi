using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.Modals;

/// <summary>
/// Admin modals
/// </summary>
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
    /// Add or refresh a Guild Wars 2 account
    /// </summary>
    /// <param name="modalData">Modal input</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [ModalInteraction("modal;admin;docker;create")]
    public Task AddOrRefreshGuildWarsAccount(CreateDockerContainerModalData modalData) => CommandHandler.CreateNewContainer(Context, modalData);

    #endregion // Methods

}