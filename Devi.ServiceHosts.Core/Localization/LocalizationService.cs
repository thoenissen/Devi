using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Core.Localization.Data;
using Devi.ServiceHosts.Core.ServiceProvider;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace Devi.ServiceHosts.Core.Localization;

/// <summary>
/// Providing located string
/// </summary>
[Injectable<LocalizationService>(ServiceLifetime.Singleton)]
public class LocalizationService : ISingletonInitialization
{
    #region Fields

    /// <summary>
    /// Groups
    /// </summary>
    private ConcurrentDictionary<string, LocalizationGroup> _groups;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Culture
    /// </summary>
    public CultureInfo Culture { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Loading the located data
    /// </summary>
    /// <param name="stream">Stream</param>
    public void Load(Stream stream)
    {
        var data = JsonConvert.DeserializeObject<LocalizationData>(new StreamReader(stream).ReadToEnd());

        Culture = CultureInfo.CreateSpecificCulture(data?.CultureInfo ?? "en-US");

        if (data?.TranslationGroups != null)
        {
            _groups = new ConcurrentDictionary<string, LocalizationGroup>(data.TranslationGroups.ToDictionary(obj => obj.Key, obj => new LocalizationGroup(this, obj.Value)));
        }
        else
        {
            _groups = new ConcurrentDictionary<string, LocalizationGroup>();
        }
    }

    /// <summary>
    /// Return a group by the given key
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>The <see cref="LocalizationGroup"/> matching the given key.</returns>
    public LocalizationGroup GetGroup(string key)
    {
        if (_groups?.TryGetValue(key, out var group) != true)
        {
            group = new LocalizationGroup(this);
        }

        return group;
    }

    #endregion // Methods

    #region ISingletonInitialization

    /// <summary>
    /// Initialize
    /// </summary>
    /// <remarks>When this method is called all services are registered and can be resolved.  But not all singleton services may be initialized. </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Initialize()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly != null)
        {
            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.Languages.de-DE.json");
            if (stream != null)
            {
                await using (stream.ConfigureAwait(false))
                {
                    Load(stream);
                }
            }
        }
    }

    #endregion // ISingletonInitialization
}