using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Player commands
/// </summary>
[Injectable<PlayerCommandHandler>(ServiceLifetime.Transient)]
public class PlayerCommandHandler : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Web API connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Web API connector</param>
    public PlayerCommandHandler(LocalizationService localizationService,
                                WebApiConnector connector)
        : base(localizationService)
    {
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Add character
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="characterName">Name</param>
    /// <param name="characterClass">Class</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task AddCharacter(InteractionContextContainer context, string characterName, Class characterClass)
    {
        await context.DeferProcessing(true)
                     .ConfigureAwait(false);

        await _connector.PenAndPaper
                        .AddCharacter(context.Channel.Id,
                                      context.User.Id,
                                      characterName,
                                      characterClass)
                        .ConfigureAwait(false);

        await context.DeleteOriginalResponse()
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Remove character
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task RemoveCharacter(InteractionContextContainer context)
    {
        await context.DeferProcessing(true)
                     .ConfigureAwait(false);

        await _connector.PenAndPaper
                        .RemoveCharacter(context.Channel.Id,
                                         context.User.Id)
                        .ConfigureAwait(false);

        await context.DeleteOriginalResponse()
                     .ConfigureAwait(false);
    }

    #endregion // Methods
}