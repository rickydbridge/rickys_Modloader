using System.Web;
using System.Text.Json;

//TODO: Clean up this namespace and class a little bit
//TODO: Make comments a little more helpful
//TODO: Add summary comments and other C# snippits things as this is meant to work as a pseudo-library
namespace rickyd.Modloader.ApiRequestTool
{
    class ApiRequester(string apiUrl)
    {
        // Use one instance of an HttpClient for optimization
        static readonly HttpClient httpClient = new();
        static readonly JsonSerializerOptions options = new() { WriteIndented = true };

        // Base URL for the Modrinth API that we are accessing
        public string url = apiUrl;

        public async Task<HttpResponseMessage> Get (string[] requestArgs)
        {
            if (requestArgs.Length.Equals(0))
            {
                Debug.Err("API Request Error:  No API request was included");
                return new();
            }
            
            // Build API Request URI from base url and additional provided arguments

            // Initial url should be the base url plus the additional request path given first in the arguments array
            UriBuilder uriBuilder = new(url + requestArgs[0]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            // For each argument (beyond the request path) add it to the queryParams array
            for (int i = 1; i < requestArgs.Length; i++)
            {
                try {
                    string[] argument = requestArgs[i].Split('=');
                    query[argument[0]] = argument[1];
                } catch { continue; }
            }
            // Create the request url from the parameters
            uriBuilder.Query = query.ToString();
            string requestUrl = uriBuilder.ToString();
            

            try {
                
                // Send GET request asynchronously
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                // Check if the HTTP status code of the response is successful
                response.EnsureSuccessStatusCode();

                // Display the response as a formatted JSON text
                await DisplayJson(response, requestArgs[0]);

                return response;

            } catch (HttpRequestException err) {
                // If there's a request error, display it
                Debug.Warn($"Request error: {err.Message}");
                throw new UnsuccessfulApiRequest();
            }
        }

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
                formattedJson = JsonSerializer.Serialize(doc.RootElement, options);
            } catch (JsonException){
                // If the text fails to be formatted, return the original body
                formattedJson = responseBody;
            }

            //Write the formatted text, with line breaks preceding and following
            Debug.Log(name + "  =>  " + formattedJson);
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