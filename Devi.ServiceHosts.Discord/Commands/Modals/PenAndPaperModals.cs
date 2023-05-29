using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.Modals;

/// <summary>
/// Pen and paper modals
/// </summary>
public class PenAndPaperModals : LocatedInteractionModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public PenAndPaperCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="modalData">Modal input</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [ModalInteraction("modal;pnp;campaign;create")]
    public Task CreateCampaign(CreateCampaignModalData modalData) => CommandHandler.CreateCampaign(Context, modalData);

    #endregion // Methods

}