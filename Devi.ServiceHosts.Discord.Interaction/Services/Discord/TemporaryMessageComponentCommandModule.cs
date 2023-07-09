﻿using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Interaction.Commands.Base;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Devi.ServiceHosts.Discord.Interaction.Services.Discord;

/// <summary>
/// General module for processing temporary message components
/// </summary>
public class TemporaryMessageComponentCommandModule : LocatedInteractionModuleBase
{
    #region Methods

    /// <summary>
    /// Temporary button
    /// </summary>
    /// <param name="identification">Identification</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("temporary;button;*")]
    public async Task ExecuteTemporaryButton(string identification)
    {
        if (((IInteractionContext)Context).Interaction is SocketMessageComponent component)
        {
            Context.Interactivity.CheckButtonComponent(identification, component);
        }
        else
        {
            await Context.DeferAsync()
                         .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Temporary select menu
    /// </summary>
    /// <param name="identification">Identification</param>
    /// <param name="unused">Unused</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("temporary;selectMenu;*")]
    public async Task ExecuteTemporarySelectMenu(string identification, string[] unused)
    {
        if (((IInteractionContext)Context).Interaction is SocketMessageComponent component)
        {
            Context.Interactivity.CheckSelectMenuComponent(identification, component);
        }
        else
        {
            await Context.DeferAsync()
                         .ConfigureAwait(false);
        }
    }

    #endregion // Methods
}