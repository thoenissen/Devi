using Discord;
using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.Modals.Data;

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
}