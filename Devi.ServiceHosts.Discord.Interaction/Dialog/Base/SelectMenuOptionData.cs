﻿using Discord;

namespace Devi.ServiceHosts.Discord.Interaction.Dialog.Base;

/// <summary>
/// Select menu option
/// </summary>
public class SelectMenuOptionData
{
    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Label
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Emote
    /// </summary>
    public IEmote Emote { get; set; }
}