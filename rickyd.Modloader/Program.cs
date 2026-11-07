

namespace rickyd.Modloader
{
    class Program
    {
        public static Context context = new(DateTime.Now);

        public static async Task Main (string[] args)
        {
            // Create the logs folder if it doesn't exist, then create a log file for the existing context.
            if (!Path.Exists(@".\logs"))
            {
                Directory.CreateDirectory(@".\logs");
            }
            File.AppendAllText(context.pathToLog, $"Starting rickyd.Modloader at {context.startTimestamp:F}.\n");

        }
    }

    class Context(DateTime currentTimestamp)
    {
        public readonly DateTime startTimestamp = currentTimestamp;
        public readonly string pathToLog = @".\logs\" + currentTimestamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
    }
}
