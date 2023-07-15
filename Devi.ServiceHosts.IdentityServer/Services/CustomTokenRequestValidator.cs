using System.Security.Claims;

using Duende.IdentityServer.Validation;

namespace Devi.ServiceHosts.IdentityServer.Services;

/// <summary>
/// Adding custom data to tokens
/// </summary>
public sealed class CustomTokenRequestValidator : ICustomTokenRequestValidator
{
    #region Fields

    /// <summary>
    /// Client ID
    /// </summary>
    private static readonly string _clientId = Environment.GetEnvironmentVariable("DEVI_WEBAPI_CLIENT_ID");

    #endregion // Fields

    #region ICustomTokenRequestValidator

    /// <summary>
    /// Custom validation logic for a token request.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The validation result</returns>
    public Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
        if (context.Result != null
         && context.Result.ValidatedRequest.ClientId == _clientId)
        {
            context.Result.ValidatedRequest.Client.ClientClaimsPrefix = string.Empty;
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("role", "InternalService"));
        }

        return Task.CompletedTask;
    }

    #endregion // ICustomTokenRequestValidator
}