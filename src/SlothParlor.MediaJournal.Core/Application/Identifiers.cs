using System.Security.Claims;
using SlothParlor.MediaJournal.Core.Extensions;

public static class Identifiers
{
    public static string CreateUserId(ClaimsPrincipal claimsPrincipal) => 
        CreateUserId(claimsPrincipal.GetIdp(), claimsPrincipal.GetObjectId());

    public static string CreateUserId(string idp, string objectId) => $"{idp}/{objectId}";
}