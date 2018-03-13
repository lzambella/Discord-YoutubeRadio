using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FacebookSharp.GraphAPI.ApiParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
/*
 * TODO:
 * Every field can only be obtained on demand (meaning a user has to select either none or any of the fields when grabbing data)
 * The current way is to load those fields into a list and generate a string thats a part of the url (such as ?parameters=1,2,3,4).
 * This works because we don't have to parse the values and convert them to valid fields but it also backfires because we give
 * the user total control resulting in unspecified behavior is something is entered wrong. Find a way to perhaps let the user easily
 * get the parameters and use them (maybe an enum will work?)
 */
namespace FacebookSharp.GraphAPI
{
    /// <summary>
    /// This represents a Facebook Page.
    /// </summary>
    public class Page : IProfile
    {
        /// <summary>
        /// Page ID. No access token is required to access this field
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Information about the Page
        /// </summary>
        [JsonProperty("about")]
        public string About { get; set; }
        /// <summary>
        /// The access token you can use to act as the Page. Only visible to Page Admins. If your business requires two-factor authentication, and the person hasn't authenticated, this field may not be returned
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// The Page's currently running promotion campaign
        /// </summary>
        [JsonProperty("ad_campaign")]
        public AdCampaign AdCampaign { get; set; }
        [JsonProperty("affilition")]
        public string Affilition { get; set; }
        [JsonProperty("app_id")]
        public string Appid { get; set; }
        //TODO: Implement AppLinks object
        [JsonProperty("app_links")]
        public object AppLinks { get; set; }
        [JsonProperty("artists_we_like")]
        public string ArtistsWeLike { get; set; }
        [JsonProperty("attire")]
        public string Attire { get; set; }
        [JsonProperty("awards")]
        public string Awards { get; set; }
        [JsonProperty("band_interests")]
        public string BandInterests { get; set; }
        [JsonProperty("band_members")]
        public string BandMembers { get; set; }
        [JsonProperty("best_page")]
        public Page BestPage { get; set; }
        [JsonProperty("bio")]
        public string Bio { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
        [JsonProperty("booking_agent")]
        public string BookingAgent { get; set; }
        //TODO: Is this a business object?
        [JsonProperty("business")]
        public Business Business { get; set; }
        [JsonProperty("can_checkin")]
        public bool CanCheckin { get; set; }
        [JsonProperty("can_post")]
        public bool CanPost { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        //TODO: implement PageCategory
        [JsonProperty("category_list")]
        public object CategoryList { get; set; }
        [JsonProperty("checkins")]
        public uint CheckIns { get; set; }
        [JsonProperty("company_overview")]
        public string CompanyOverview { get; set; }
        // TODO: implement MailingAddress
        [JsonProperty("contact_address")]
        public object MailingAddress { get; set; }
        [JsonProperty("context")]
        public object Context { get; set; }
        [JsonProperty("cover")]
        public object Cover { get; set; }
        [JsonProperty("culinary_team")]
        public string CulinaryTeam { get; set; }
        [JsonProperty("current_location")]
        public string CurrentLocation { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("description_html")]
        public string DescriptionHtml { get; set; }
        [JsonProperty("directed_by")]
        public string DirectedBy { get; set; }
        [JsonProperty("display_subtext")]
        public string DisplaySubtext { get; set; }
        [JsonProperty("displayed_message_response_time")]
        public string DisplayedMessageRespponseTime { get; set; }
        [JsonProperty("emails")]
        public IList<string> Emails { get; set; }
        [JsonProperty("engagement")]
        public object Engagement { get; set; }
        [JsonProperty("fan_count")]
        public uint FanCount { get; set; }
        [JsonProperty("featured_video")]
        public object FeaturedVideo { get; set; }
        [JsonProperty("features")]
        public string Features { get; set; }
        [JsonProperty("food_styles")]
        public IList<string> FoodStyles { get; set; }
        [JsonProperty("founded")]
        public string Founded { get; set; }
        [JsonProperty("general_info")]
        public string GeneralInfo { get; set; }
        [JsonProperty("general_manager")]
        public string GeneralManager { get; set; }
        [JsonProperty("genre")]
        public string Genre { get; set; }
        [JsonProperty("global_brand_page_name")]
        public string GlobalBrandPageName { get; set; }
        [JsonProperty("global_brand_root_id")]
        public string GlobalBrandRootId { get; set; }
        [JsonProperty("has_added_app")]
        public bool HasAddedApp { get; set; }
        [JsonProperty("hometown")]
        public string Hometown { get; set; }
        [JsonProperty("hours")]
        public IDictionary<string, string> Hours { get; set; }
        [JsonProperty("impressum")]
        public string Impressum { get; set; }
        [JsonProperty("influences")]
        public string Influences { get; set; }
        [JsonProperty("instant_articles_review_status")]
        public int InstantArticlesReviewStatus { get; set; }
        [JsonProperty("is_always_open")]
        public bool IsAlwaysOpen { get; set; }
        [JsonProperty("is_community_page")]
        public bool IsCommunityPage { get; set; }
        [JsonProperty("is_permanently_close")]
        public bool IsPermanentlyClosed { get; set; }
        [JsonProperty("is_published")]
        public bool IsPublished { get; set; }
        [JsonProperty("is_unclaimed")]
        public bool IsUnclaimed { get; set; }
        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }
        [JsonProperty("is_webhooks_subscribed")]
        public bool IsWebhooksSubscribed { get; set; }
        [JsonProperty("leadgen_tos_accepted")]
        public bool LeadgenTosAccepted { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        //TODO
        [JsonProperty("location")]
        public object Location { get; set; }
        [JsonProperty("members")]
        public string Members { get; set; }
        [JsonProperty("mission")]
        public string Mission { get; set; }
        [JsonProperty("mpg")]
        public string Mpg { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("name_with_location_descriptor")]
        public string NameWithLocationDescriptor { get; set; }
        [JsonProperty("network")]
        public string Network { get; set; }
        [JsonProperty("new_like_count")]
        public uint NewLikeCount { get; set; }
        [JsonProperty("offer_eligible")]
        public bool OfferEligible { get; set; }
        [JsonProperty("overall_star_rating")]
        public float OverallStarRating { get; set; }
        [JsonProperty("parent_page")]
        public Page ParentPage { get; set; }
        [JsonProperty("parking")]
        public object Parking { get; set; }
        [JsonProperty("payment_options")]
        public object PaymentOptions { get; set; }
        [JsonProperty("personal_info")]
        public string PersonalInfo { get; set; }
        [JsonProperty("personal_interests")]
        public string PersonalInterests { get; set; }
        [JsonProperty("pharma_safety_info")]
        public string PharmaSafetyInfo { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("place_type")]
        public int PlaceType { get; set; }
        [JsonProperty("plot_outline")]
        public string PlotOutline { get; set; }
        [JsonProperty("preferred_audience")]
        public object PreferredAudience { get; set; }
        [JsonProperty("press_contact")]
        public string PressContact { get; set; }
        [JsonProperty("price_range")]
        public string PriceRange { get; set; }
        [JsonProperty("produced_by")]
        public string ProducedBy { get; set; }
        [JsonProperty("products")]
        public string Products { get; set; }
        [JsonProperty("promotion_eligible")]
        public bool PromotionEligible { get; set; }
        [JsonProperty("promotion_ineligible_reason")]
        public string PromotionIneligiblReason { get; set; }
        [JsonProperty("public_transit")]
        public string PublicTransit { get; set; }
        [JsonProperty("publisher_space")]
        public object PropertySpace { get; set; }
        [JsonProperty("rating_count")]
        public uint RatingCount { get; set; }
        [JsonProperty("recipient")]
        public string Recipient { get; set; }
        [JsonProperty("record_label")]
        public string RecordLabel { get; set; }
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty("restaurant_services")]
        public object RestaurantServices { get; set; }
        [JsonProperty("restaurant_specialties")]
        public object RestaurantSpecialties { get; set; }
        [JsonProperty("schedule")]
        public string Schedule { get; set; }
        [JsonProperty("screenplay_by")]
        public string ScreenplayBy { get; set; }
        [JsonProperty("season")]
        public string Season { get; set; }
        [JsonProperty("single_line_address")]
        public string SingleLineAddress { get; set; }
        [JsonProperty("starring")]
        public string Starring { get; set; }
        [JsonProperty("start_info")]
        public object StartInfo { get; set; }
        [JsonProperty("store_location_descriptor")]
        public string StoreLocationDescriptor { get; set; }
        [JsonProperty("store_number")]
        public uint StoreNumber { get; set; }
        [JsonProperty("studio")]
        public string Studio { get; set; }
        [JsonProperty("supports_instant_articles")]
        public bool SupportsInstantArticles { get; set; }
        [JsonProperty("talking_about_count")]
        public string TalkingAboutCount { get; set; }
        [JsonProperty("unread_message_count")]
        public uint UnreadMessageCount { get; set; }
        [JsonProperty("unread_notif_count")]
        public uint UnreadNotificationCount { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("verification_status")]
        public string VerificationStatus { get; set; }
        [JsonProperty("voip_info")]
        public object VoipInfo { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("were_here_count")]
        public uint WereHereCount { get; set; }
        [JsonProperty("written_by")]
        public string WrittenBy { get; set; }

        // Everything ahead is all the edges a page has


        [Obsolete("Same things as GetPhotos(false), exists to prevent rewrite of tests.")]
        public async Task<PagePhotos> GetPhotos()
        {
            return await GetPhotos(false);
        }

        /// <summary>
        /// Gets the Photo edge of a page
        /// </summary>
        /// <param name="uploaded">true: uploaded images on page</param>
        /// <returns>Photo object containing list of IDs and Names</returns>
        public async Task<PagePhotos> GetPhotos(bool uploaded)
        {
            var v = "v2.8";
            var url = $"https://graph.facebook.com/{v}/{Id}/photos?access_token={GraphApi.Token}";
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response == null)
                return null;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = await sr.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<PagePhotos>(json);
                return data;
            }
        }

        /// <summary>
        /// Gets Photo edge of a Page containing data pertaining to fields input
        /// </summary>
        /// <param name="fields">Fields of data to be returned</param>
        /// <param name="uploaded">true: uploaded images on page false: profile pictures page has used</param>
        /// <returns></returns>
        public async Task<PagePhotos> GetPhotos(IFieldHelper fields, bool uploaded)
        {
            var v = "v2.8";
            var url = $"https://graph.facebook.com/{v}/{Id}/photos?access_token={GraphApi.Token}&{fields.GenerateFields()}";
            if (uploaded)
                url = $"https://graph.facebook.com/{v}/{Id}/photos?access_token={GraphApi.Token}&{fields.GenerateFields()}&type=uploaded";
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response == null)
                return null;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = await sr.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<PagePhotos>(json);
                return data;
            }
        }
    }
}
