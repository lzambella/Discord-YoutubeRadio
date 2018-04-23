using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class ReachAndFrequencySpec
    {
        [JsonProperty("countries")]
        public IList<string> Countries { get; set; }
        [JsonProperty("min_campaign_duration")]
        public string MinCampaignDuration { get; set; }
        [JsonProperty("max_campaign_duration")]
        public string MaxCampaignDuration { get; set; }
        [JsonProperty("max_days_to_finish")]
        public string MaxDaysToFinish { get; set; }
        [JsonProperty("min_reach_limit")]
        public string MinDaysToFinish { get; set; }
    }
}
