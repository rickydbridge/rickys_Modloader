
namespace rickyd.Modloader
{
    //TODO: Make multiple line logs indented for readability
    class Debug
    {
        public static void Log (string? text, bool verbose = false)
        {
            // Don't attempt to log if the provided text is null
            if (text is null)
                return;
            
            string fullText = text;
            
            // Count the number of lines of text. If greater than ten, show only the first ten lines, unless specified to be verbose
            int numLines = 1 + text.Count(c => c.Equals('\n'));
            if (numLines >= 10 && !verbose)
            {
                text = string.Join("\n", text.Split('\n')[0..10]) + "\n        . . .  [ Only the first 10 lines are displayed. ]";
            }

            // Write the lines to the console and to the current log file stored in program context
            string prefixedText = $"[{DateTime.Now:u} ==>   LOG]    {text}";
            string prefixedFullText = $"[{DateTime.Now:u} ==>   LOG]    {fullText}";
            Console.WriteLine(prefixedText);
            File.AppendAllText(Program.context.pathToLog, "\n" + prefixedFullText);
        }
        public static void Warn (string? text, bool verbose = false)
        {
            // Don't attempt to log if the provided text is null
            if (text is null)
                return;
            
            string fullText = text;

            // Count the number of lines of text. If greater than ten, show only the first ten lines, unless specified to be verbose
            int numLines = 1 + text.Count(c => c.Equals('\n'));
            if (numLines >= 10 && !verbose)
            {
                text = string.Join("\n", text.Split('\n')[0..10]) + "\n        . . .  [ Only the first 10 lines are displayed. ]";
            }

            // Write the lines to the console and to the current log file stored in program context
            string prefixedText = $"[{DateTime.Now:u} ==>  WARN]    {text}";
            string prefixedFullText = $"[{DateTime.Now:u} ==> WARN]    {fullText}";
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(prefixedText);
            Console.ResetColor();
            File.AppendAllText(Program.context.pathToLog, "\n" + prefixedFullText);
        }
        public static void Err (string? text, bool verbose = false)
        {
            // Don't attempt to log if the provided text is null
            if (text is null)
                return;
            
            string fullText = text;

            // Count the number of lines of text. If greater than ten, show only the first ten lines, unless specified to be verbose
            int numLines = 1 + text.Count(c => c.Equals('\n'));
            if (numLines >= 10 && !verbose)
            {
                text = string.Join("\n", text.Split('\n')[0..10]) + "\n        . . .  [ Only the first 10 lines are displayed. ]";
            }

            // Write the lines to the console and to the current log file stored in program context
            string prefixedText = $"[{DateTime.Now:u} ==> ERROR]    {text}";
            string prefixedFullText = $"[{DateTime.Now:u} ==> ERROR]    {fullText}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(prefixedText);
            Console.ResetColor();
            File.AppendAllText(Program.context.pathToLog, "\n" + prefixedFullText);
        }
    }
}
