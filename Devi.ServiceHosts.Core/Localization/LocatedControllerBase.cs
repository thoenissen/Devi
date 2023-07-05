using Microsoft.AspNetCore.Mvc;

namespace Devi.ServiceHosts.Core.Localization;

/// <summary>
/// Command module base class with localization services
/// </summary>
public abstract class LocatedControllerBase : ControllerBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    protected LocatedControllerBase(LocalizationService localizationService)
    {
        LocalizationGroup = localizationService?.GetGroup(GetType().Name);
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Localization group
    /// </summary>
    public LocalizationGroup LocalizationGroup { get; }

    #endregion // Properties
}