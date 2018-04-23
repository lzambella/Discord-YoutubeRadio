using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookSharp.GraphAPI
{
    public class User : IProfile
    {
        /// <summary>
        /// The id of this person's user account. This ID is unique to each app and cannot be used across different apps. 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Equivalent to the bio field
        /// </summary>
        [JsonProperty("about")]
        public string About { get; set; }
        [JsonProperty("admin_notes")]
        public IList<PageAdminNote> AdminNotes { get; set; }
        // TODO: implement AgeRange
        [JsonProperty("age_range")]
        public object AgeRange { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
        [JsonProperty("context")]
        public object UserContext { get; set; }
        [JsonProperty("cover")]
        public object CoverPhoto { get; set; }
        [JsonProperty("devices")]
        public IList<object> Devices { get; set; }
        [JsonProperty("education")]
        public IList<object> Education { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("employee_number")]
        public string EmployeeNumber { get; set; }
        [JsonProperty("favorite_athletes")]
        public IList<object> FavoriteAthletes { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("name_format")]
        public string NameFormat { get; set; }
        [JsonProperty("payment_pricepoints")]
        public object PaymentPricepoints { get; set; }
        [JsonProperty("political")]
        public string PoliticalViews { get; set; }
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
        [JsonProperty("quotes")]
        public string Quotes { get; set; }
        [JsonProperty("relationship_status")]
        public string RelationshipStatus { get; set; }
        [JsonProperty("religion")]
        public string Religion { get; set; }
        [JsonProperty("security_settings")]
        public object SecuritySettings { get; set;  }
        [JsonProperty("shared_login_upgrade_required_by")]
        public string SharedLoginUpgradeRequiredBy { get; set; }
        [JsonProperty("significant_other")]
        public User SignificantOther { get; set; }
        [JsonProperty("sports")]
        public IList<object> Sports { get; set; }
        [JsonProperty("test_group")]
        public UInt32 TestGroup { get; set; }
        [JsonProperty("third_party_id")]
        public string ThirdPartyId { get; set; }
        [JsonProperty("timezone")]
        public float Timezone { get; set; }
        [JsonProperty("token_for_business")]
        public string TokenForBusiness { get; set; }
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("video_upload_limits")]
        public object VideoUploadLimits { get; set; }
        [JsonProperty("viewer_can_send_gift")]
        public bool ViewerCanSendGift { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("work")]
        public IList<object> Work { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }

    }
}
