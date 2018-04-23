using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// You use an agency client declaration to describe the clients represented by an agency.
    /// </summary>
    public class AgencyClientDeclaration
    {
        //TODO: refactor this
        /// <summary>
        /// Whether this account is for an agency representing a client
        /// </summary>
        [JsonProperty("agency_representing_client")]
        public int AgencyRepresentingClient { get; set; }
        /// <summary>
        /// Whether the client is based in France
        /// </summary>
        [JsonProperty("client_based_in_france")]
        public int ClientBasedInFrance { get; set; }
        [JsonProperty("client_city")]
        public string ClientCity { get; set; }
        [JsonProperty("client_country_code")]
        public string ClientCountryCode { get; set; }
        [JsonProperty("client_email_address")]
        public string ClientEmailAddress { get; set; }
        [JsonProperty("client_name")]
        public string ClientName { get; set; }
        [JsonProperty("client_postal_code")]
        public string ClientPostalCode { get; set; }
        [JsonProperty("client_province")]
        public string ClientProvince { get; set; }
        [JsonProperty("client_street")]
        public string ClientStreet { get; set; }
        [JsonProperty("client_street2")]
        public string ClientStreet2 { get; set; }
        [JsonProperty("has_written_mandate_from_advertiser")]
        public int HasWrittenMandateFromAdvertiser { get; set; }
        [JsonProperty("is_client_paying_invoices")]
        public int IsClientPayingInvoices { get; set; }
    }
}
