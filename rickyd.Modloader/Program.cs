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

        public static async Task Main (string[] args)
        {
            try
            {


                // Create the program's folders if they doesn't exist, then create a log file for the existing context.
                Directory.CreateDirectory(@".\logs");
                Directory.CreateDirectory(@".\bin\downloads\mods");
                File.AppendAllText(context.pathToLog, $"Starting rickyd.Modloader at {context.startTimestamp:F}.\n");

                await DownloadModsFromList(args[0]);


            }
            catch (Exception ex) { Debug.Err(ex.ToString()); throw; }
        }

        private static async Task DownloadModsFromList(string path)
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
            Debug.Log($"Parsing modlist from file {Path.GetFullPath(path)}");
            string[] parsedLines = File.ReadAllLines(path);
            Debug.Log(string.Join("\n", parsedLines));
            return parsedLines;
        }
    }

    class Context(DateTime currentTimestamp)
    {
        public readonly DateTime startTimestamp = currentTimestamp;
        public readonly string pathToLog = @".\logs\" + currentTimestamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
        public readonly JsonSerializerOptions jsonFormat = new() { WriteIndented = true };
    }
}
