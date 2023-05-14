using Devi.ServiceHosts.Core.Localization;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Services.Discord;

/// <summary>
/// Interaction module base class with localization services
/// </summary>
public class LocatedInteractionModuleBase : InteractionModuleBase<InteractionContextContainer>
{
    #region Fields

    /// <summary>
    /// Localization group
    /// </summary>
    private LocalizationGroup _localizationGroup;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Localization service
    /// </summary>
    public LocalizationService LocalizationService { protected get; set; }

    /// <summary>
    /// Localization group
    /// </summary>
    public LocalizationGroup LocalizationGroup => _localizationGroup ??= LocalizationService.GetGroup(GetType().Name);

    #endregion // Properties

    #region ModuleBase

    /// <summary>
    /// Before execution
    /// </summary>
    /// <param name="command">Command</param>
    public override void BeforeExecute(ICommandInfo command)
    {
        Context.Command = command;
    }

    #endregion // ModuleBase
}