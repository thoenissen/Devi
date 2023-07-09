using Discord;
using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;

/// <summary>
/// Creation of a docker container
/// </summary>
public class CreateDockerContainerModalData : IModal
{
    /// <summary>
    /// Title
    /// </summary>
    public string Title => "Container creation";

    /// <summary>
    /// Name
    /// </summary>
    [InputLabel("Name")]
    [RequiredInput]
    [ModalTextInput(nameof(Name), TextInputStyle.Short, "Devi.Discord")]
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [InputLabel("Description")]
    [RequiredInput]
    [ModalTextInput(nameof(Description), TextInputStyle.Short, "Devi - Discord bot")]
    public string Description { get; set; }
}