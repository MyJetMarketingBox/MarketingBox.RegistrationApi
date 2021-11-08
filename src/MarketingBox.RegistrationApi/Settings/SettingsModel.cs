using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.RegistrationApi.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxRegistrationApi.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationApi.RegistrationServiceUrl")]
        public string RegistrationServiceUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationApi.JaegerUrl")]
        public string JaegerUrl { get; set; }
    }
}
