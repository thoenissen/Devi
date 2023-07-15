using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Devi.ServiceHosts.IdentityServer.Pages.Account.Login;

/// <summary>
/// Login page
/// </summary>
[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    #region Methods

    /// <summary>
    /// Get route
    /// </summary>
    /// <param name="returnUrl">Return url</param>
    /// <returns>Redirect result to external provider</returns>
    public IActionResult OnGet(string returnUrl)
    {
        return RedirectToPage("/ExternalLogin/Challenge", new { scheme = "Discord", returnUrl });
    }

    #endregion // Methods
}