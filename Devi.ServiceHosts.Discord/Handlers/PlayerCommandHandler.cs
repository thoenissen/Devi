using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Player commands
/// </summary>
public class PlayerCommandHandler : LocatedServiceBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public PlayerCommandHandler(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Add character
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public Task AddCharacter(InteractionContextContainer context)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Remove character
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public Task RemoveCharacter(InteractionContextContainer context)
    {
        throw new System.NotImplementedException();
    }

    #endregion // Methods
}