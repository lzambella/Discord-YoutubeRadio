using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FacebookSharp.GraphAPI.ApiParameters
{
    public class UserField : IFieldHelper
    {
        public IList<UserFields> Fields { get; set; }
        public UserField()
        {
            Fields = new List<UserFields>();
        }
        /// <summary>
        /// Fields available to the User object
        /// </summary>
        public enum UserFields
        {
            // Using the description attribute is a clever but clunky way of getting the actual JSON value without using switch statements during url generation
            [Description("id")]
            Id,
            [Description("about")]
            About,
            [Description("admin_notes")]
            AdminNotes,
            [Description("age_range")]
            AgeRange,
            [Description("birthday")]
            Birthday,
            [Description("context")]
            Context,
            [Description("cover")]
            Cover,
            [Description("currency")]
            Currency,
            [Description("devices")]
            Devices,
            [Description("education")]
            Education,
            [Description("email")]
            Email,
            [Description("employee_number")]
            EmployeeNumber,
            [Description("favorite_atheletes")]
            FavoriteAtheletes,
            [Description("favorite_teams")]
            FavoriteTeams,
            [Description("first_name")]
            FirstName,
            [Description("gender")]
            Gender,
            [Description("hometown")]
            Hometown,
            [Description("inspirational_people")]
            InspirationalPeople,
            [Description("install_type")]
            InstallType,
            [Description("installed")]
            Installed,
            [Description("interested_in")]
            InterestedIn,
            [Description("is_shared_login")]
            IsSharedLogin,
            [Description("is_verified")]
            IsVerified,
            [Description("labels")]
            Labels,
            [Description("languages")]
            Languages,
            [Description("last_name")]
            LastName,
            [Description("link")]
            Link,
            [Description("locale")]
            Locale,
            [Description("location")]
            Location,
            [Description("meeting_for")]
            MeetingFor,
            [Description("middle_name")]
            MiddleName,
            [Description("name")]
            Name,
            [Description("name_format")]
            NameFormat,
            [Description("payment_pricepoints")]
            PaymentPricepoints,
            [Description("political")]
            Political,
            [Description("public_key")]
            PublicKey,
            [Description("quotes")]
            Quotes,
            [Description("relationship_status")]
            RelationshipStatus,
            [Description("religion")]
            Religion,
            [Description("security_settings")]
            SecuritySettings,
            [Description("shared_login_upgrade_required_by")]
            SharedLoginUpgradeRequiredBy,
            [Description("significant_other")]
            SignificantOther,
            [Description("sports")]
            Sports,
            [Description("test_group")]
            TestGroup,
            [Description("third_party_ids")]
            ThirdPartyId,
            [Description("timezone")]
            Timezone,
            [Description("token_for_business")]
            TokenForBusiness,
            [Description("updated_time")]
            UpdatedTime,
            [Description("verified")]
            Verified,
            [Description("video_upload_limit")]
            VideoUploadLimits,
            [Description("viewer_can_send_gift")]
            ViewerCanSendGift,
            [Description("website")]
            Website,
            [Description("work")]
            Work
        }
        /// <summary>
        /// Generates the specified fields and puts them into a format readable by the graph api.
        /// Uses API requests
        /// </summary>
        /// <returns></returns>
        public string GenerateFields()
        {
            var strOutput = "fields=";
            for (var i = 0; i < Fields.Count; i++)
            {
                strOutput += EnumHelper.GetEnumDescription(Fields[i]);
                if (i < Fields.Count - 1)
                    strOutput += ',';
            }
            return strOutput;
        }
    }
}
