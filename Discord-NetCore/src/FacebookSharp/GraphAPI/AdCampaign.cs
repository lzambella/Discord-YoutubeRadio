using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    //TODO: can json be deserialized into enums
    /// <summary>
    /// An ad set is a group of ads that share the same daily or lifetime budget, schedule, bid type, bid info, and targeting data. Ad sets enable you to group ads according to your criteria, and you can retrieve the ad-related statistics that apply to a set.
    /// Prior to July 2014 ad sets were referred to as 'campaigns'. When using ad sets in API calls the parameter may be referred to as 'adcampaign'. A campaign contains one or more ad sets.
    /// </summary>
    public class AdCampaign
    {
        public enum ConfigStatus
        {
            ACTIVE,
            PAUSED,
            DELETED,
            ARCHIVE
        }

        public enum EffStatus
        {
            ACTIVE, PAUSED, DELETED, PENDING_REVIEW, DISAPPROVED, PREAPPROVED, PENDING_BILLING_INFO, CAMPAIGN_PAUSED, ARCHIVED, ADSET_PAUSED
        }

        public enum Stats
        {
            ACTIVE, PAUSED, DELETED, ARCHIVED
        }
        /// <summary>
        /// Ad set ID
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }
        /// <summary>
        /// Ad Account ID
        /// </summary>
        [JsonProperty("account_id")]
        string AccountId { get; set; }
        [JsonProperty("adlabels")]
        public IList<AdLabel> AdLabels { get; set; }
        [JsonProperty("brand_lift_studies")]
        public List<AdStudy> BrandLiftStudies { get; set; }
        [JsonProperty("budget_rebalance_flag")]
        public bool BudgetRebalanceFlag { get; set; }
        [JsonProperty("buying_type")]
        public string BuyingType { get; set; }
        [JsonProperty("can_create_brand_lift_study")]
        public bool CanCreateBrandLiftStudy { get; set; }
        [JsonProperty("can_use_spend_cap")]
        public bool CanUseSpendCap { get; set; }
        [JsonProperty("configured_status")]
        public ConfigStatus ConfiguredStatus { get; set; }
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        [JsonProperty("effective_status")]
        public EffStatus EffectiveStatus { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("objective")]
        public string Objective { get; set; }
        [JsonProperty("recommendations")]
        public IList<AdRecommendation> Recommendations { get; set; }
        [JsonProperty("spend_cap")]
        public string SpendCap { get; set; }
        [JsonProperty("start_time")]
        public string StartTime { get; set; }
        [JsonProperty("status")]
        public Stats Status { get; set; }
        [JsonProperty("stop_time")]
        public string StopTime { get; set; }
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }

    }
}
