using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Dialog.Base;
using Devi.ServiceHosts.Discord.Dialog.Configuration;
using Devi.ServiceHosts.Discord.Services.Discord;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Configuration commands
/// </summary>
public class ConfigurationCommandHandler : LocatedServiceBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public ConfigurationCommandHandler(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Server configuration
    /// </summary>
    /// <param name="context">Context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Configure(InteractionContextContainer context)
    {
        using (var dialogHandler = new DialogHandler(context))
        {
            await dialogHandler.Run<ServerConfigurationDialogElement, bool>()
                               .ConfigureAwait(false);

            await dialogHandler.DeleteMessages()
                               .ConfigureAwait(false);
        }
    }

    #endregion // Methods
}