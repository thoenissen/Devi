﻿using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.MessageComponents;

/// <summary>
/// Admin component commands
/// </summary>
public class AdminMessageComponentCommandModule : LocatedInteractionModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public AdminCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Commands

    /// <summary>
    /// Create docker container
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("admin;docker;create")]
    public Task CreateDockerContainer() => CommandHandler.CreateNewContainer(Context);

    /// <summary>
    /// Refresh containers
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("admin;docker;refresh")]
    public Task RefreshDockerContainer() => CommandHandler.RefreshContainers(Context);

    /// <summary>
    /// Show information of selected container
    /// </summary>
    /// <param name="selection">Selection</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("admin;docker;selectContainer")]
    public async Task LeadSelection(string[] selection)
    {
        await Context.DeferAsync()
                     .ConfigureAwait(false);

        if (selection?.Length > 0)
        {
            await CommandHandler.ShowSelectContainer(Context, selection[0])
                                .ConfigureAwait(false);
        }
    }

    #endregion // Commands
}