using System.CodeDom;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using rickyd.Modloader.ApiRequestTool;

//TODO: Add UI
namespace rickyd.Modloader
{
    class Program
    {
        public static Context context = new(DateTime.Now);
        // Use one instance of an HttpClient
        public static readonly HttpClient httpClient = new();
        public static ApiRequester api = new("https://api.modrinth.com/v2/");

        [STAThread]
        public static void Main (string[] args)
        {
            try
            {


                // Create the program's folders if they doesn't exist, then create a log file for the existing context.
                Directory.CreateDirectory(@".\logs");
                Directory.CreateDirectory(@".\bin\downloads\mods");
                File.AppendAllText(context.pathToLog, $"Starting rickyd.Modloader at {context.startTimestamp:F}.\n");

                Application.Run(new UI.MainWindow());

                //await DownloadModsFromList(args[0]);


            }
            catch (Exception ex) { Debug.Err(ex.ToString()); throw; }
        }

        public static async Task DownloadModsFromList(string path)
        {
            string[] modlist = ParseModlist(path);
            string[] loaderContext = modlist[0].Split(" ");

            for (int i = 1; i < modlist.Length; i++)
            {
                // Get the mod versions
                HttpResponseMessage response = await api.Get([$"/project/{modlist[i]}/version", $"loaders=[\"{loaderContext[0]}\"]", $"game_versions=[\"{loaderContext[1]}\"]", "include_changelog=false"]);
                // Parse the JSON to identify the latest version (as a precaution in case the versions aren't pre-sorted)
                JsonArray json = JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsArray();
                JsonNode latestVersion = json.OrderByDescending(node => node!["date_published"]!.GetValue<DateTime>()).First()!;
                Debug.Log($"Latest version of {modlist[i]} is -> {JsonSerializer.Serialize(latestVersion, context.jsonFormat)}");
                // Next, identify the primary file for the mod
                JsonNode primaryFile = latestVersion["files"]!.AsArray().OrderByDescending(node => node!["primary"]!.GetValue<bool>()).First()!;
                // Finally, get the cdn for the mod file and stream it into a file 
                string url = primaryFile["url"]!.GetValue<string>();
                string filename = primaryFile["filename"]!.GetValue<string>();
                byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync($@".\bin\downloads\mods\{filename}", fileBytes);
            }
        }

        /// <summary>
        /// Parses a modlist file made up of a list of modrinth slugs.
        /// </summary>
        /// <param name="path">The path to the modlist file</param>
        /// <returns>A string array, where the first two elements are the modloader context, and every subsequent element is a modrinth slug.</returns>
        public static string[] ParseModlist (string path)
        {
            try
            {
                Debug.Log($"Parsing modlist from file {Path.GetFullPath(path)}");
                // Parse all lines into an array
                string[] parsedLines = File.ReadAllLines(path);
                // Loader context is stored on the first line in format: LOADER X.X
                string[] loaderContext = parsedLines[0].Split(" ");
                // If the loader context is not one of the known types, throw an exception
                // This technique catches files that would otherwise parse without exception to prevent them from reaching the ApiRequester
                if (!new string[] { "fabric", "forge", "neoforge" }.Contains(loaderContext[0]))
                    throw new UnsuccessfulModlistParse($"Loader context {{{loaderContext[0]}}} does not match any known form (eg. fabric, forge, etc.).");
                return parsedLines;
            } catch (Exception ex)
            {
                // If the exception caught is already of the right type, just rethrow it.
                // Otherwise, we put it inside an UnsuccessfulModlistParse exception for additional context as to what caused the failure.
                if (ex is UnsuccessfulModlistParse)
                    throw;
                throw new UnsuccessfulModlistParse($"Modlist failed to parse due to unforseen exception: {ex.Message}", ex);
            }
        }
    }

    public class UnsuccessfulModlistParse : Exception
    {
        public UnsuccessfulModlistParse ()
        {}

        public UnsuccessfulModlistParse (string message) 
            : base(message)
        {}

        public UnsuccessfulModlistParse (string message, Exception innerException)
            : base (message, innerException)
        {}
    }

    class Context(DateTime currentTimestamp)
    {
        public readonly DateTime startTimestamp = currentTimestamp;
        public readonly string pathToLog = @".\logs\" + currentTimestamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
        public readonly JsonSerializerOptions jsonFormat = new() { WriteIndented = true };
    }
}
