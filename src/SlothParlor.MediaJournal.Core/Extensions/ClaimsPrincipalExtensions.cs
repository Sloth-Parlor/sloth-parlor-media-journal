using System.Security.Claims;

namespace SlothParlor.MediaJournal.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetAppUserId(this ClaimsPrincipal principal) => 
        Identifiers.CreateUserId(principal);

    public static string GetObjectId(this ClaimsPrincipal principal) => 
        GetRequiredClaim(principal, Authentication.UserClaimTypes.ObjectId);

    public static string GetIdp(this ClaimsPrincipal principal) => 
        GetRequiredClaim(principal, Authentication.UserClaimTypes.Idp);

    public static string GetPrimaryEmail(this ClaimsPrincipal principal) => 
        GetRequiredClaim(principal, Authentication.UserClaimTypes.Emails);

    public static string GetRequiredClaim(ClaimsPrincipal principal, string claimType)
        => GetClaim(principal, claimType) 
        ?? throw new InvalidOperationException($"Missing required claim '{claimType}'");

    public static string? GetClaim(ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value;
}