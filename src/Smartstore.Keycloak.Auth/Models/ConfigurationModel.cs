namespace Smartstore.Keycloak.Auth.Models
{
    [LocalizedDisplay("Plugins.ExternalAuth.Keycloak.")]
    public class ConfigurationModel : ModelBase
    {
        [LocalizedDisplay("*Authority")]
        public string Authority { get; set; }

        [LocalizedDisplay("*Realm")]
        public string Realm { get; set; }

        [LocalizedDisplay("*ClientId")]
        public string ClientId { get; set; }

        [LocalizedDisplay("*ClientSecret")]
        public string ClientSecret { get; set; }

        [LocalizedDisplay("*RedirectUri")]
        public string RedirectUrl { get; set; }
    }
}
