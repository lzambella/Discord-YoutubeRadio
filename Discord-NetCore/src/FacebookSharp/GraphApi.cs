using System.IO;
using System.Net;
using System.Threading.Tasks;
using FacebookSharp.GraphAPI;
using FacebookSharp.GraphAPI.ApiParameters;
using Newtonsoft.Json;

namespace FacebookSharp
{
    /// <summary>
    /// Main object, contains all the top level objects (user, page, photo, etc.)
    /// </summary>
    public class GraphApi
    {
        /// <summary>
        /// Determines which api version to use
        /// </summary>
        public enum ApiVersion
        {
            TwoEight,
        }

        public static string Token { get; set; }
        public ApiVersion Version { get; set; }

        /// <summary>
        /// Initiate a new GraphAPI object
        /// </summary>
        /// <param name="token">Facebook Graph API Token</param>
        /// <param name="version">API version to use</param>
        public GraphApi(string token, ApiVersion version)
        {
            Token = token;
            Version = version;
        }
        /// <summary>
        /// Gets the string value of the API version contained in Version
        /// </summary>
        /// <returns>String of the API version (I.E. 'v2.8')</returns>
        public string GetVersion()
        {
            switch (Version)
            {
                case ApiVersion.TwoEight:
                    return "v2.8";
            }
            return "";
        }
        /// <summary>
        /// Gets the specified place.
        /// </summary>
        /// <param name="id">ID of the place.</param>
        /// <returns>A Place</returns>
        /*
        public async Task<Place> GetPlace(string id, ApiField fields = null)
        {
            var json = "";
            if (fields == null) json = await GetJson(id);
            else json = await GetJson(id, fields);
            return JsonConvert.DeserializeObject<Place>(json);
        }
        */
        /// <summary>
        /// Gets a User
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="fields">Fields to grab from (such as description, date of birth...)</param>
        /// <returns>User object containing data related to the fields inputted</returns>
        public async Task<User> GetUser(string id, UserField fields = null)
        {
            var json = "";
            if (fields == null)
                json = await GetJson(id);
            else
                json = await GetJson(id, fields);
            return JsonConvert.DeserializeObject<User>(json);
        }

        /// <summary>
        /// Gets the page of a facebook user with a specific ID
        /// </summary>
        /// <param name="id">ID of the user to get from</param>
        /// <param name="fields">fields to get from</param>
        /// <returns>Page object containing data (such as description, date, name)</returns>
        public async Task<Page> GetPage(string id, IFieldHelper fields = null)
        {
            var json = "";
            if (fields == null)
                json = await GetJson(id);
            else
                json = await GetJson(id, fields);
            return JsonConvert.DeserializeObject<Page>(json);
        }

        /// <summary>
        /// Gets json from an api call
        /// </summary>
        /// <param name="id">ID parameter</param>
        /// <returns>JSON string containing API data, varies depending on which type is accessed, ID always gets returned</returns>
        private async Task<string> GetJson(string id)
        {
            var http = $"https://graph.facebook.com/{GetVersion()}/{id}?access_token={Token}";
            var request = WebRequest.Create(http);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response == null)
                return null;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = await sr.ReadToEndAsync();
                return json;
            }
        }

        /// <summary>
        /// Get JSON data
        /// </summary>
        /// <param name="id">ID of the page</param>
        /// <param name="fields">fields to grab</param>
        /// <returns>JSON string containing API data</returns>
        private async Task<string> GetJson(string id, IFieldHelper fields)
        {
            var http = $"https://graph.facebook.com/{GetVersion()}/{id}?{fields.GenerateFields()}&access_token={Token}";
            var request = WebRequest.Create(http);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response == null)
                return null;

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = await sr.ReadToEndAsync();
                return json;
            }
        }
    }

}
