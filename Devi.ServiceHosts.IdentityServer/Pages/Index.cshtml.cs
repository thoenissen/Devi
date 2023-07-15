using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Devi.ServiceHosts.IdentityServer.Pages;

/// <summary>
/// Main page
/// </summary>
[AllowAnonymous]
public class Index : PageModel
{
}