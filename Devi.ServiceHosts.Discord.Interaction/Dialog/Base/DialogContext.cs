using System.Collections.Generic;

using Discord;

namespace Devi.ServiceHosts.Discord.Interaction.Dialog.Base;

/// <summary>
/// Dialog context
/// </summary>
public class DialogContext
{
    #region Fields

    /// <summary>
    /// Values
    /// </summary>
    private Dictionary<string, object> _values;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public DialogContext()
    {
        _values = new Dictionary<string, object>();
        Messages = new List<IMessage>();
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Message
    /// </summary>
    public List<IMessage> Messages { get; }

    /// <summary>
    /// Should the responses be ephemeral?
    /// </summary>
    public bool UseEphemeralMessages { get; set; }

    /// <summary>
    /// Should the messages modify the current reply instead of posting new messages?
    /// </summary>
    public bool ModifyCurrentMessage { get; set; }

    /// <summary>
    /// Set value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public void SetValue<T>(string key, T value)
    {
        _values[key] = value;
    }

    /// <summary>
    /// Get value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <returns>Value</returns>
    public T GetValue<T>(string key)
    {
        return (T)_values[key];
    }

    #endregion // Properties
}