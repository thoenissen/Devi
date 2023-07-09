using Discord;
using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;

/// <summary>
/// Creation of a campaign
/// </summary>
public class CreateCampaignModalData : IModal
{
    /// <summary>
    /// Title
    /// </summary>
    public string Title => "Campaign creation";

    /// <summary>
    /// Name
    /// </summary>
    [InputLabel("Name")]
    [RequiredInput]
    [ModalTextInput(nameof(Name), TextInputStyle.Short, "A House Divided")]
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [InputLabel("Description")]
    [RequiredInput]
    [ModalTextInput(nameof(Description), TextInputStyle.Paragraph, "A House Divided is a tale of gothic horror and familial drama. The adventure features immersive...")]
    public string Description { get; set; }

    /// <summary>
    /// Day
    /// </summary>
    [InputLabel("Day of week")]
    [RequiredInput]
    [ModalTextInput(nameof(Day), TextInputStyle.Short, "Mon|Tue|Wed|Thu|Fri|Sat|Sun")]
    public string Day { get; set; }

    /// <summary>
    /// Day
    /// </summary>
    [InputLabel("Time of day")]
    [RequiredInput]
    [ModalTextInput(nameof(Time), TextInputStyle.Short, "hh:mm")]
    public string Time { get; set; }
}