using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Dungeon master commands
/// </summary>
public class DungeonMasterCommandHandler : LocatedServiceBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public DungeonMasterCommandHandler(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public Task CreateCampaign(InteractionContextContainer context) => throw new System.NotImplementedException();

    #endregion // Methods
}