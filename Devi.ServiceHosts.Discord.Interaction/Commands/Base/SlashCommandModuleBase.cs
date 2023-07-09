using System.Collections.Generic;
using System.Linq;

using Devi.ServiceHosts.Discord.Interaction.Extensions;
using Devi.ServiceHosts.Discord.Interaction.Services.Discord;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Base;

/// <summary>
/// SlashCommand base class
/// </summary>
public abstract class SlashCommandModuleBase : LocatedInteractionModuleBase
{
    #region LocatedInteractionModuleBase

    /// <summary>
    /// Creates a list of all commands
    /// </summary>
    /// <remarks>Only the <see cref="SlashCommandBuildContext"/> is available and not the command context during this method.</remarks>
    /// <param name="buildContext">Build context</param>
    /// <returns>List of commands</returns>
    public virtual IEnumerable<ApplicationCommandProperties> GetCommands(SlashCommandBuildContext buildContext) => buildContext.ServiceProvider
                                                                                                                               .GetRequiredService<DiscordClient>()
                                                                                                                               .Interaction
                                                                                                                               .Modules
                                                                                                                               .FirstOrDefault(obj => obj.Name == GetType().Name)
                                                                                                                               ?.ToApplicationCommandProps();

    #endregion // LocatedInteractionModuleBase
}