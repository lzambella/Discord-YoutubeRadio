using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        string id { get; set; }
        /// <summary>
        /// The ID of the ad account
        /// </summary>
        string account_id { get; set; }
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
        Int32 account_status { get; set; }
        /// <summary>
        /// Amount of time the ad account has been open, in days
        /// </summary>
        float age { get; set; }
        /// <summary>
        /// Details of the agency advertising on behalf of this client account, if applicable
        /// </summary>
        AgencyClientDeclaration agency_client_declaration { get; set; }
        /// <summary>
        /// Current total amount spent by the account. This can be reset.
        /// </summary>
        string amount_spent { get; set; }
        /// <summary>
        /// The attribution specification of ad account, including click through window length, view through window length, etc.
        /// </summary>
        IList<object> attribution_spec { get; set; }
        /// <summary>
        /// Bill amount due
        /// </summary>
        string balance { get; set; }
        /// <summary>
        /// The Business Manager, if this ad account is owned by one.
        /// </summary>
        Business business { get; set; }
        /// <summary>
        /// City for business address
        /// </summary>
        string business_city { get; set; }
        /// <summary>
        /// Country code for the business address
        /// </summary>
        string business_county_code { get; set; }
        /// <summary>
        /// The business name for the account
        /// </summary>
        string business_name { get; set; }
        /// <summary>
        /// First line of the business street address for the account
        /// </summary>
        string business_street { get; set; }
        /// <summary>
        /// Second line of the business street address for the account
        /// </summary>
        string business_street2 { get; set; }
        /// <summary>
        /// Zip code for business address
        /// </summary>
        string business_zip { get; set; }
        /// <summary>
        /// If we can create a new automated brand lift study under the ad account.
        /// </summary>
        bool can_create_brand_lift_study { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IList<string> capabilities { get; set; }
        /// <summary>
        /// The time the account was created in ISO 8601 format.
        /// </summary>
        DateTime created_time { get; set; }
        /// <summary>
        /// The currency used for the account, based on the corresponding value in the account settings. 
        /// </summary>
        string currency { get; set; }
        UInt32 disable_reason { get; set; }
        string end_advertiser { get; set; }
        string end_advertiser_name { get; set; }
        //continue from falied_delivery_checks
    }
}
