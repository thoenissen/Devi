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
    [ModalTextInput(nameof(Description), TextInputStyle.Paragraph, "A House Divided is a tale of gothic horror and familial drama. The adventure features immersive exploration and tactically challenging combat, but its primary gameplay emphasis is social interaction with a rich cast of nuanced characters. What choices will the party make within the halls of Raventree Estate? What allegiances will they forge and what betrayals will they commit? Will the heroes restore the manor to its former glory, release it from its torment, or seize its power for their own?")]
    public string Description { get; set; }
}