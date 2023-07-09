using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Discord;
using Discord.WebSocket;

using Newtonsoft.Json;

namespace Devi.ServiceHosts.Discord.Interaction.Services.Discord;

/// <summary>
/// Providing emoji
/// </summary>
public static class DiscordEmoteService
{
    #region Fields

    /// <summary>
    /// Emotes
    /// </summary>
    private static readonly ConcurrentDictionary<string, ulong> _emotes;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    static DiscordEmoteService()
    {
        _emotes = new ConcurrentDictionary<string, ulong>(JsonConvert.DeserializeObject<Dictionary<string, ulong>>(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Devi.ServiceHosts.Discord.Interaction.Resources.Emotes.json")).ReadToEnd()));
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Get 'Loading'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetLoadingEmote(IDiscordClient client) => GetEmote(client, "Loading");

    /// <summary>
    /// Get 'Gold'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetGoldEmote(IDiscordClient client) => GetEmote(client, "Gold");

    /// <summary>
    /// Get 'Silver'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetSilverEmote(IDiscordClient client) => GetEmote(client, "Silver");

    /// <summary>
    /// Get 'Copper'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetCopperEmote(IDiscordClient client) => GetEmote(client, "Copper");

    /// <summary>
    /// Get 'Check'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetCheckEmote(IDiscordClient client) => GetEmote(client, "Check");

    /// <summary>
    /// Get 'Cross'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetCrossEmote(IDiscordClient client) => GetEmote(client, "Cross");

    /// <summary>
    /// Get 'Artificer'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetArtificerEmote(IDiscordClient client) => GetEmote(client, "Artificer");

    /// <summary>
    /// Get 'Barbarian'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetBarbarianEmote(IDiscordClient client) => GetEmote(client, "Barbarian");

    /// <summary>
    /// Get 'Bard'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetBardEmote(IDiscordClient client) => GetEmote(client, "Bard");

    /// <summary>
    /// Get 'Cleric'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetClericEmote(IDiscordClient client) => GetEmote(client, "Cleric");

    /// <summary>
    /// Get 'Druid'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetDruidEmote(IDiscordClient client) => GetEmote(client, "Druid");

    /// <summary>
    /// Get 'Fighter'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetFighterEmote(IDiscordClient client) => GetEmote(client, "Fighter");

    /// <summary>
    /// Get 'Monk'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetMonkEmote(IDiscordClient client) => GetEmote(client, "Monk");

    /// <summary>
    /// Get 'Paladin'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetPaladinEmote(IDiscordClient client) => GetEmote(client, "Paladin");

    /// <summary>
    /// Get 'Ranger'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetRangerEmote(IDiscordClient client) => GetEmote(client, "Ranger");

    /// <summary>
    /// Get 'Rogue'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetRogueEmote(IDiscordClient client) => GetEmote(client, " Rogue");

    /// <summary>
    /// Get 'Sorcerer'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetSorcererEmote(IDiscordClient client) => GetEmote(client, "Sorcerer");

    /// <summary>
    /// Get 'Warlock'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetWarlockEmote(IDiscordClient client) => GetEmote(client, "Warlock");

    /// <summary>
    /// Get 'Wizard'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetWizardEmote(IDiscordClient client) => GetEmote(client, "Wizard");

    /// <summary>
    /// Get 'Add'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetAddEmote(IDiscordClient client) => GetEmote(client, "Add");

    /// <summary>
    /// Get 'Add2'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetAdd2Emote(IDiscordClient client) => GetEmote(client, "Add2");

    /// <summary>
    /// Get 'Edit'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEditEmote(IDiscordClient client) => GetEmote(client, "Edit");

    /// <summary>
    /// Get 'Edit2'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEdit2Emote(IDiscordClient client) => GetEmote(client, "Edit2");

    /// <summary>
    /// Get 'Edit3'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEdit3Emote(IDiscordClient client) => GetEmote(client, "Edit3");

    /// <summary>
    /// Get 'Edit4'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEdit4Emote(IDiscordClient client) => GetEmote(client, "Edit4");

    /// <summary>
    /// Get 'Edit5'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEdit5Emote(IDiscordClient client) => GetEmote(client, "Edit5");

    /// <summary>
    /// Get 'TrashCan'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetTrashCanEmote(IDiscordClient client) => GetEmote(client, "TrashCan");

    /// <summary>
    /// Get 'TrashCan2'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetTrashCan2Emote(IDiscordClient client) => GetEmote(client, "TrashCan2");

    /// <summary>
    /// Get 'Emote'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEmojiEmote(IDiscordClient client) => GetEmote(client, "Emoji");

    /// <summary>
    /// Get 'Empty'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetEmptyEmote(IDiscordClient client) => GetEmote(client, "Empty");

    /// <summary>
    /// Get 'Bullet'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetBulletEmote(IDiscordClient client) => GetEmote(client, "Bullet");

    /// <summary>
    /// Get 'Image'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetImageEmote(IDiscordClient client) => GetEmote(client, "Image");

    /// <summary>
    /// Get 'QuestionMark'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetQuestionMarkEmote(IDiscordClient client) => GetEmote(client, "QuestionMark");

    /// <summary>
    /// Get 'ArrowUp'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetArrowUpEmote(IDiscordClient client) => GetEmote(client, "ArrowUp");

    /// <summary>
    /// Get 'ArrowDown'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetArrowDownEmote(IDiscordClient client) => GetEmote(client, "ArrowDown");

    /// <summary>
    /// Get 'Star'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetStarEmote(IDiscordClient client) => GetEmote(client, "Star");

    /// <summary>
    /// Get 'First'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetFirstEmote(IDiscordClient client) => GetEmote(client, "First");

    /// <summary>
    /// Get 'Previous'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GePreviousEmote(IDiscordClient client) => GetEmote(client, "Previous");

    /// <summary>
    /// Get 'Next'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetNextEmote(IDiscordClient client) => GetEmote(client, "Next");

    /// <summary>
    /// Get 'Last'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetLastEmote(IDiscordClient client) => GetEmote(client, "Last");

    /// <summary>
    /// Get guild emoji
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <param name="id">Id</param>
    /// <returns>Emote</returns>
    public static IEmote GetGuildEmote(IDiscordClient client, ulong id)
    {
        IEmote emote = null;

        try
        {
            if (client is BaseSocketClient socketClient)
            {
                emote = socketClient.Guilds
                                    .SelectMany(obj => obj.Emotes)
                                    .FirstOrDefault(obj => obj.Id == id);
            }
        }
        catch
        {
        }

        return emote ?? Emoji.Parse(":grey_question:");
    }

    /// <summary>
    /// Get emoji by the given key
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <param name="key">key</param>
    /// <returns>Emote</returns>
    public static IEmote GetEmote(IDiscordClient client, string key)
    {
        IEmote emote = null;

        try
        {
            if (_emotes.TryGetValue(key, out var emojiId))
            {
                emote = GetGuildEmote(client, emojiId);
            }
        }
        catch
        {
        }

        return emote ?? Emoji.Parse(":grey_question:");
    }

    #endregion // Methods
}