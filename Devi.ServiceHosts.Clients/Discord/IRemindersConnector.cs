﻿using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Reminders;

namespace Devi.ServiceHosts.Clients.Discord;

/// <summary>
/// Reminders connector
/// </summary>
public interface IRemindersConnector
{
    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PostOneTimeReminder(PostReminderMessageDTO dto);
}