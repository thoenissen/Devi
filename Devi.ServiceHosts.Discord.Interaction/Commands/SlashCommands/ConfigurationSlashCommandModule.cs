using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.SlashCommands;

/// <summary>
/// Server configuration commands
/// </summary>
[Injectable<ConfigurationSlashCommandModule>(ServiceLifetime.Transient)]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[DontAutoRegister]
public class ConfigurationSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public ConfigurationCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Server configuration
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("configuration", "Server configuration")]
    public Task Configure() => CommandHandler.Configure(Context);

    #endregion // Methods
}