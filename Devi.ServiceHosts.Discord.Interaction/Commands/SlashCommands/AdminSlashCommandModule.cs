using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.SlashCommands;

/// <summary>
/// Reminder commands
/// </summary>
[Injectable<AdminSlashCommandModule>(ServiceLifetime.Transient)]
[DontAutoRegister]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("admin", "Administration")]
public class AdminSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public AdminCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Docker
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("docker-edit", "Docker")]
    public Task ShowDockerContainerAssistant() => CommandHandler.ShowDockerContainerAssistant(Context);

    /// <summary>
    /// Docker
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("docker-overview", "Docker")]
    public Task ShowDockerContainerOverview() => CommandHandler.ShowDockerContainerOverview(Context);

    #endregion // Methods
}