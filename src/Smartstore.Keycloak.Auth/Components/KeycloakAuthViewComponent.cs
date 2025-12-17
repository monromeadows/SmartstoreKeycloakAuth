using Microsoft.AspNetCore.Mvc;
using Smartstore.Web.Components;

namespace Smartstore.Keycloak.Auth.Components
{
    public class KeycloakAuthViewComponent : SmartViewComponent
    {
        private readonly OpenIdConnectOptions _keycloakOptions;

        public KeycloakAuthViewComponent(IOptionsMonitor<OpenIdConnectOptions> keycloakOptions)
        {
            _keycloakOptions = keycloakOptions.Get(KeycloakDefaults.AuthenticationScheme);
        }

        public IViewComponentResult Invoke()
        {
            if (string.IsNullOrEmpty(_keycloakOptions.Authority) || 
                string.IsNullOrEmpty(_keycloakOptions.ClientId) || 
                string.IsNullOrEmpty(_keycloakOptions.ClientSecret))
            {
                return Empty();
            }

            var returnUrl = HttpContext.Request.Query["returnUrl"].ToString();
            var href = Url.Action("ExternalLogin", "Identity", new { provider = "Keycloak", returnUrl });
            var title = T("Plugins.ExternalAuth.Keycloak.Login").Value;
            var html = $"<a class='btn btn-primary btn-block btn-lg btn-extauth btn-brand-keycloak' href='{href}' rel='nofollow'>" +
                       $"<i class='fas fa-fw fa-lg fa-key' aria-hidden='true'></i><span>{title}</span></a>";

            return HtmlContent(html);
        }
    }
}
