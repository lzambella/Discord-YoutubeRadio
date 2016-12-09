using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// This represents a Facebook Page.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Page ID. No access token is required to access this field
        /// </summary>
        string id { get; set; }
        /// <summary>
        /// Information about the Page
        /// </summary>
        string about { get; set; }
        /// <summary>
        /// The access token you can use to act as the Page. Only visible to Page Admins. If your business requires two-factor authentication, and the person hasn't authenticated, this field may not be returned
        /// </summary>
        string access_token { get; set; }
        /// <summary>
        /// The Page's currently running promotion campaign
        /// </summary>
        AdCampaign ad_campaign { get; set; }
        // TODO: Finish AdCampaign
    }
}
