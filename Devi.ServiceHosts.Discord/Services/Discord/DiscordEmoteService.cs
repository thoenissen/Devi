using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Discord;
using Discord.WebSocket;

using Newtonsoft.Json;

namespace Devi.ServiceHosts.Discord.Services.Discord;

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
        _emotes = new ConcurrentDictionary<string, ulong>(JsonConvert.DeserializeObject<Dictionary<string, ulong>>(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Devi.ServiceHosts.Discord.Resources.Emotes.json")).ReadToEnd()));
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
    /// Get 'Gunslinger'-Emote
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns>Emote</returns>
    public static IEmote GetGunslingerEmote(IDiscordClient client) => GetEmote(client, "Gunslinger");

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