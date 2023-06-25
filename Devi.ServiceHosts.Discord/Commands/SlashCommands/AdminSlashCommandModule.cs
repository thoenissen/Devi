using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord;
using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Reminder commands
/// </summary>
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