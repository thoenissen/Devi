using Discord;

namespace Devi.ServiceHosts.Discord.Interaction.Extensions;

/// <summary>
/// <see cref="IUser"/>
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Gets the best display name
    /// </summary>
    /// <param name="user">Member</param>
    /// <returns>Display name</returns>
    public static string TryGetDisplayName(this IUser user)
    {
        if (user is IGuildUser member)
        {
            if (string.IsNullOrWhiteSpace(member.Nickname) == false)
            {
                return member.Nickname;
            }

            if (string.IsNullOrWhiteSpace(member.DisplayName) == false)
            {
                return member.DisplayName;
            }
        }

        return user?.Username;
    }
}