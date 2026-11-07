using rickyd.Modloader.ApiRequestTool;

namespace rickyd.Modloader
{
    class Program
    {
        public static Context context = new(DateTime.Now);
        public static ApiRequester api = new("https://api.modrinth.com/v2/");

        public static async Task Main (string[] args)
        {
            // Create the logs folder if it doesn't exist, then create a log file for the existing context.
            if (!Path.Exists(@".\logs"))
            {
                Directory.CreateDirectory(@".\logs");
            }
            File.AppendAllText(context.pathToLog, $"Starting rickyd.Modloader at {context.startTimestamp:F}.\n");

            await api.Get(args);
        }
    }

    class Context(DateTime currentTimestamp)
    {
        public readonly DateTime startTimestamp = currentTimestamp;
        public readonly string pathToLog = @".\logs\" + currentTimestamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
    }
}
