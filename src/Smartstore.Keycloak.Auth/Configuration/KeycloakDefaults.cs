namespace Smartstore.Keycloak.Auth
{
    /// <summary>
    /// Default values for Keycloak authentication.
    /// </summary>
    public static class KeycloakDefaults
    {
        /// <summary>
        /// The default authentication scheme for Keycloak.
        /// </summary>
        public const string AuthenticationScheme = "Keycloak";

        /// <summary>
        /// The default callback path for Keycloak OIDC authentication.
        /// </summary>
        public const string CallbackPath = "/signin-keycloak";
    }
}
