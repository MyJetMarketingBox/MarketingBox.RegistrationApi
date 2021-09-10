﻿
using MarketingBox.AfffliateApi.Models.CampaignBoxes;

namespace MarketingBox.RegistrationApi.Models.CampaignBoxes
{
    public class CampaignBoxModel
    {
        public long CampaignBoxId { get; set; }
        public long BoxId { get; set; }
        public long CampaignId { get; set; }
        public string CountryCode { get; set; }
        public int Priority { get; set; }
        public int Weight { get; set; }
        public CapType CapType { get; set; }

        public long DailyCapValue { get; set; }
        public ActivityHours[] ActivityHours { get; set; }
        public string Information { get; set; }
        public bool EnableTraffic { get; set; }
    }
}