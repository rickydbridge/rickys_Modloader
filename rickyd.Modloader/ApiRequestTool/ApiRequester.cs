using System.Web;
using System.Text.Json;

namespace rickyd.Modloader.ApiRequestTool
{
    /// <summary>
    /// Class containing asyncronous methods for making HTTP GET requests from a given API url.
    /// </summary>
    /// <param name="apiUrl">Base URL for the API being referneced</param>
    class ApiRequester(string apiUrl)
    {

        // Base URL for the API we are referencing, passed in on construction
        public string url = apiUrl;

        /// <summary>
        /// Calls an HTTP GET request to the API.
        /// </summary>
        /// <param name="requestArgs">Arguments for the API reference. 0th item is the path, each subsequent item is a key=value pair of query parameters.</param>
        /// <returns>HttpResponseMessage from the HTTP GET request</returns>
        public async Task<HttpResponseMessage> Get (string[] requestArgs)
        {
            // If there are no arguments passed in, throw an UnsuccessfulApiRequest exception.
            if (requestArgs is null || requestArgs.Length.Equals(0))
            {
                Debug.Err("ApiRequester Error:  No api request added.");
                throw new UnsuccessfulApiRequest("No api request added.");
            }


            /// =====================
            /// Build API Request URI
            /// =====================

            // Initial url should be the base url plus the additional request path given first in the arguments array
            UriBuilder uriBuilder = new(url + requestArgs[0]);
            // Parse the initial input for uri queries, and start the list with any that are present
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            // For each argument (beyond the request path) add it to the queryParams array
            for (int i = 1; i < requestArgs.Length; i++)
            {
                try {
                    string[] argument = requestArgs[i].Split('=');
                    query[argument[0]] = argument[1];
                } catch {
                    Debug.Warn($"ApiRequester Warning: Argument {requestArgs[i]} was not in expected key=value format.");
                    continue;
                }
            }
            // Create the request url from the parameters
            uriBuilder.Query = query.ToString();
            string requestUrl = uriBuilder.ToString();


            /// ==============================
            /// Make HTTP GET Request from API
            /// ==============================

            try {
                
                // Send GET request asynchronously
                HttpResponseMessage response = await Program.httpClient.GetAsync(requestUrl);

                // Check if the HTTP status code of the response is successful
                response.EnsureSuccessStatusCode();

                // Display the response as a formatted JSON text
                await DisplayJson(response, requestArgs[0]);

                return response;

            } catch (HttpRequestException err) {
                // If there's a request error, display it
                Debug.Err(err.ToString());
                throw new UnsuccessfulApiRequest($"HttpRequestException when making API Request {requestArgs[0]}\n ", err);
            }
        }

        //TODO: Take a look at this. Ideally it parses JSON to be worked with, and could then be displayed whenever.
        public static async Task DisplayJson (HttpResponseMessage response, string name="")
        {
            string formattedJson;
            // Get the response as an unformatted string
            string responseBody = await response.Content.ReadAsStringAsync();

            // Format the string into proper JSON
            try {
                // Turn the string into a JSON doc
                using JsonDocument doc = JsonDocument.Parse(responseBody);

                // Serialize with indented formatting
                formattedJson = JsonSerializer.Serialize(doc.RootElement, Program.context.jsonFormat);
            } catch (JsonException){
                // If the text fails to be formatted, return the original body
                formattedJson = responseBody;
            }

            //Write the formatted text, with line breaks preceding and following
            Debug.Log($"API GET {name}  ->  {formattedJson}");
        }
    }

    [Serializable]
    public class UnsuccessfulApiRequest : Exception
    {
        public UnsuccessfulApiRequest ()
        {}

        public UnsuccessfulApiRequest (string message) 
            : base(message)
        {}

        public UnsuccessfulApiRequest (string message, Exception innerException)
            : base (message, innerException)
        {}    
    }
}
