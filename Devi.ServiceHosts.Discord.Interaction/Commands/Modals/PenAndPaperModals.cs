using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals;

/// <summary>
/// Pen and paper modals
/// </summary>
[Injectable<PenAndPaperModals>(ServiceLifetime.Transient)]
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