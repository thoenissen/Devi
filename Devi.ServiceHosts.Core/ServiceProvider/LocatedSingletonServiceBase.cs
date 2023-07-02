using Devi.ServiceHosts.Core.Localization;

namespace Devi.ServiceHosts.Core.ServiceProvider;

/// <summary>
/// Base class for singleton services
/// </summary>
public class LocatedSingletonServiceBase
{
    #region Fields

    /// <summary>
    /// Localization service
    /// </summary>
    private LocalizationService _localizationService;

    /// <summary>
    /// Localization group
    /// </summary>
    private LocalizationGroup _localizationGroup;

    #endregion // Fields

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public LocatedSingletonServiceBase(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    #region Properties

    /// <summary>
    /// Localization group
    /// </summary>
    public LocalizationGroup LocalizationGroup => _localizationGroup ??= _localizationService.GetGroup(GetType().Name);

    #endregion // Properties
}