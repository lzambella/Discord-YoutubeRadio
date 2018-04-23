using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// An ad account is an account used for managing ads on Facebook.
    /// </summary>
    public class AdAccount
    {
        /// <summary>
        /// The string act_{ad_account_id}
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// The ID of the ad account
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        /// <summary>
        /// Status of the account 
        /// 1 = ACTIVE
        /// 2 = DISABLED
        /// 3 = UNSETTLED
        /// 7 = PENDING_RISK_REVIEW
        /// 9 = IN_GRACE_PERIOD
        /// 100 = PENDING_CLOSURE
        /// 101 = CLOSED
        /// 102 = PENDING_SETTLEMENT
        /// 201 = ANY_ACTIVE
        /// 202 = ANY_CLOSED
        /// </summary>
        [JsonProperty("account_status")]
        public Int32 AccountStatus { get; set; }
        /// <summary>
        /// Amount of time the ad account has been open, in days
        /// </summary>
        [JsonProperty("age")]
        public float Age { get; set; }
        /// <summary>
        /// Details of the agency advertising on behalf of this client account, if applicable
        /// </summary>
        [JsonProperty("agency_client_declaration")]
        public AgencyClientDeclaration AgencyClientDeclaration { get; set; }
        /// <summary>
        /// Current total amount spent by the account. This can be reset.
        /// </summary>
        [JsonProperty("amount_spent")]
        public string AmountSpent { get; set; }
        /// <summary>
        /// The attribution specification of ad account, including click through window length, view through window length, etc.
        /// </summary>
        [JsonProperty("attribution_spec")]
        public IList<object> AttributionSpec { get; set; }
        /// <summary>
        /// Bill amount due
        /// </summary>
        [JsonProperty("balance")]
        public string Balance { get; set; }
        /// <summary>
        /// The Business Manager, if this ad account is owned by one.
        /// </summary>
        [JsonProperty("business")]
        public Business Business { get; set; }
        /// <summary>
        /// City for business address
        /// </summary>
        [JsonProperty("business_city")]
        public string BusinessCity { get; set; }
        /// <summary>
        /// Country code for the business address
        /// </summary>
        [JsonProperty("business_county_code")]
        public string BusinessCountyCode { get; set; }
        /// <summary>
        /// The business name for the account
        /// </summary>
        [JsonProperty("business_name")]
        public string BusinessName { get; set; }
        /// <summary>
        /// First line of the business street address for the account
        /// </summary>
        [JsonProperty("business_street")]
        public string BusinessStreet { get; set; }
        /// <summary>
        /// Second line of the business street address for the account
        /// </summary>
        [JsonProperty("business_street2")]
        public string BusinessStreet2 { get; set; }
        /// <summary>
        /// Zip code for business address
        /// </summary>
        [JsonProperty("business_zip")]
        public string BusinessZip { get; set; }
        /// <summary>
        /// If we can create a new automated brand lift study under the ad account.
        /// </summary>
        [JsonProperty("can_create_brand_lift_study")]
        public bool CanCreateBrandLiftStudy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("capabilities")]
        public IList<string> Capabilities { get; set; }
        /// <summary>
        /// The time the account was created in ISO 8601 format.
        /// </summary>
        //TODO: Change to DateTime
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }
        /// <summary>
        /// The currency used for the account, based on the corresponding value in the account settings. 
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("disable_reason")]
        public UInt32 DisableReason { get; set; }
        [JsonProperty("end_advertiser")]
        public string EndAdvertiser { get; set; }
        [JsonProperty("end_advertiser_name")]
        public string EndAdvertiserName { get; set; }
        [JsonProperty("failed_DeliveryChecks")]
        public IList<DeliveryChecks> FailedDelieveryChecks { get; set; }
        [JsonProperty("funcind_source")]
        public string FundingSource { get; set; }
        //TODO: Find out what this object is
        [JsonProperty("funcding_course_details")]
        public object FundingSourceDetails { get; set; }
        [JsonProperty("has_migrated_permissions")]
        public bool HasMigratedPermissions { get; set; }
        [JsonProperty("io_number")]
        public string IoNumber { get; set; }
        [JsonProperty("is_notifications_enabled")]
        public bool IsNotificationsEnabled { get; set; }
        [JsonProperty("is_personal")]
        public UInt32 IsPersonal { get; set; }
        [JsonProperty("is_prepay_account")]
        public bool IsPrepayAccount { get; set; }
        [JsonProperty("is_tax_id_required")]
        public bool IsTaxIdRequired { get; set; }
        [JsonProperty("line_numbers")]
        public IList<int> LineNumbers { get; set; }
        [JsonProperty("media_agency")]
        public string MediaAgency { get; set; }
        [JsonProperty("min_campaign_group_spend_cap")]
        public string MinCampaignGroupSpendCap { get; set; }
        [JsonProperty("min_daily_budget")]
        public UInt32 MinDailyBudget { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("offsite_pixels_tos_accepted")]
        public bool OffsitePixelsTosAccepted { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("partner")]
        public string Partner { get; set; }
        [JsonProperty("rf_spec")]
        public ReachAndFrequencySpec RfSpec { get; set; }
        [JsonProperty("salesforce_invoice_group_id")]
        public string SalesforceInvoiceGroupId { get; set; }
        [JsonProperty("show_checkout_experience")]
        public bool ShowCheckoutExperience { get; set; }
        [JsonProperty("spend_cap")]
        public string SpendCap { get; set; }
        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
        [JsonProperty("tax_id_status")]
        public UInt32 TaxIdStatus { get; set; }
        [JsonProperty("tax_id_type")]
        public string TaxIdType { get; set; }
        [JsonProperty("timezone_id")]
        public UInt32 TimezoneId { get; set; }
        [JsonProperty("timezone_name")]
        public string TimezoneName { get; set; }
        [JsonProperty("timezone_offset_hours_utc")]
        public float TimezoneOffsetHoursUtc { get; set; }
        [JsonProperty("tos_accepted")]
        public Tuple<string,int> TosAccepted { get; set; }
        [JsonProperty("user_role")]
        public string UserRole { get; set; }
    }
}
