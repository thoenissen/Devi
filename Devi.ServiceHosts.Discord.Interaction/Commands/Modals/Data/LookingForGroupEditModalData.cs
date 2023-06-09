﻿using Discord;
using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;

/// <summary>
/// Appointment edit
/// </summary>
public class LookingForGroupEditModalData : IModal
{
    /// <summary>
    /// Custom id
    /// </summary>
    public const string CustomId = "modal;lfg;edit";

    /// <summary>
    /// Creation of the appointment
    /// </summary>
    public string Title => "Appointment creation";

    /// <summary>
    /// Title
    /// </summary>
    [InputLabel("Title")]
    [RequiredInput(false)]
    [ModalTextInput(nameof(AppointmentTitle), maxLength: 95)]
    public string AppointmentTitle { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [InputLabel("Description")]
    [RequiredInput(false)]
    [ModalTextInput(nameof(AppointmentDescription), TextInputStyle.Paragraph)]
    public string AppointmentDescription { get; set; }
}