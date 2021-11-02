using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.RegistrationApi.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxRegistrationApi.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationApi.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationApi.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("MarketingBoxRegistrationApi.RegistrationServiceUrl")]
        public string RegistrationServiceUrl { get; set; }
    }
}
