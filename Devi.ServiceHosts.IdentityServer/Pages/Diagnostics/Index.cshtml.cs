using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Devi.ServiceHosts.IdentityServer.Pages.Diagnostics;

/// <summary>
/// Diagnostics page
/// </summary>
[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    #region Fields

    /// <summary>
    /// Environment
    /// </summary>
    private readonly IWebHostEnvironment _environment;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="environment">Environment</param>
    public Index(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// View model
    /// </summary>
    public ViewModel View { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Get route
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task<IActionResult> OnGet()
    {
        if (_environment.IsDevelopment() == false)
        {
            return NotFound();
        }

        View = new ViewModel(await HttpContext.AuthenticateAsync()
                                              .ConfigureAwait(false));

        return Page();
    }

    #endregion // Methods
}