namespace rickyd.Modloader
{
    class Program
    {
        public static Context context = new(DateTime.Now);

        public static async Task Main (string[] args)
        {

        }
    }

    class Context(DateTime currentTimestamp)
    {
        public readonly DateTime startTimestamp = currentTimestamp;
        public readonly string pathToLog = @".\logs\" + currentTimestamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
    }
}
