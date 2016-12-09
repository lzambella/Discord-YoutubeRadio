using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// You use an agency client declaration to describe the clients represented by an agency.
    /// </summary>
    public class AgencyClientDeclaration
    {
        /// <summary>
        /// Whether this account is for an agency representing a client
        /// </summary>
        int agency_representing_client { get; set; }
        /// <summary>
        /// Whether the client is based in France
        /// </summary>
        int client_based_in_france { get; set; }
        string client_city { get;  set; }
        string client_country_code { get; set; }
        string client_email_address { get; set; }
        string client_name { get; set; }
        string client_postal_code { get; set; }
        string client_province { get; set; }
        string client_street { get; set; }
        string client_street2 { get; set; }
        int has_written_mandate_from_advertiser { get; set; }
        int is_client_paying_invoices { get; set; }
    }
}
