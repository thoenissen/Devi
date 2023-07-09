using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.SlashCommands;

/// <summary>
/// Looking for group command handler
/// </summary>
[Injectable<LookingForGroupCommandModule>(ServiceLifetime.Transient)]
[Group("lfg", "Looking for group commands")]
public class LookingForGroupCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public LookingForGroupCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Creation of an new lfg appointment
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create", "Creation of an new lfg appointment")]
    public Task Create() => CommandHandler.StartCreation(Context);

    #endregion // Methods
}