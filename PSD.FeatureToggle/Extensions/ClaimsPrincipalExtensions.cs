using System.Security.Claims;

namespace PSD.FeatureToggle.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetEscolaId(this ClaimsPrincipal claimsPrincipal)
        {
            return int.Parse(claimsPrincipal.FindFirstValue("EscolaId"));
        }

        public static string GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Role);
        }
    }
}
