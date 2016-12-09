using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// An ad set is a group of ads that share the same daily or lifetime budget, schedule, bid type, bid info, and targeting data. Ad sets enable you to group ads according to your criteria, and you can retrieve the ad-related statistics that apply to a set.
    /// Prior to July 2014 ad sets were referred to as 'campaigns'. When using ad sets in API calls the parameter may be referred to as 'adcampaign'. A campaign contains one or more ad sets.
    /// </summary>
    public class AdCampaign
    {
        /// <summary>
        /// Ad set ID
        /// </summary>
        string id { get; set; }
        /// <summary>
        /// Ad Account ID
        /// </summary>
        string account_id { get; set; }
        //TODO: Finish AdLabel

    }
}
