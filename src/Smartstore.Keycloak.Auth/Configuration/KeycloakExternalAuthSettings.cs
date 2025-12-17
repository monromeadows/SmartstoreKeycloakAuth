using Smartstore.Core.Configuration;

namespace Smartstore.Keycloak.Auth
{
    public class KeycloakExternalAuthSettings : ISettings
    {
        /// <summary>
        /// The Keycloak server URL (e.g., https://keycloak.example.com).
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// The Keycloak realm name.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// The OAuth 2.0 Client ID configured in Keycloak.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The OAuth 2.0 Client Secret configured in Keycloak.
        /// </summary>
        public string ClientSecret { get; set; }
    }
}
