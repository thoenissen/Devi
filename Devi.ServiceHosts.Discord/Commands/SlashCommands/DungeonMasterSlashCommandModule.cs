﻿using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Dungeon master commands
/// </summary>
[Group("dm", "Dungeon master commands")]
public class DungeonMasterSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public DungeonMasterCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Campaign creation
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create-campaign", "")]
    public Task CreateCampaign() => CommandHandler.CreateCampaign(Context);

    #endregion // Methods
}