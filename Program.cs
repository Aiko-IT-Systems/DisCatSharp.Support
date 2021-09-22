using Microsoft.Extensions.Logging;

using System;
using System.Reflection;

namespace DisCatSharp.Support
{
    /// <summary>
    /// The program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            Center(" ");
            Center($"Initializing {Assembly.GetExecutingAssembly().FullName.Split(",")[0]}");
            Center(" ");
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            Console.ResetColor();

            var bot = new Bot(LogLevel.Debug);
            bot.StartAsync().Wait();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            Center(" ");
            Center($"Shutdown of {Assembly.GetExecutingAssembly().FullName.Split(",")[0]} successfull");
            Center(" ");
            Center($"Press any key to exit the aplication..");
            Center(" ");
            Console.WriteLine($"{"".PadRight(Console.WindowWidth - 2, '█')}");
            Console.ResetColor();
            Console.ReadKey(true);
            Environment.Exit(0);
        }

        /// <summary>
        /// Centers the console.
        /// </summary>
        /// <param name="s">The text.</param>
        static void Center(string s)
        {
            try
            {
                Console.Write("██");
                Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
                Console.Write(s);
                Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
                Console.WriteLine("██");
            }
            catch (Exception)
            {
                s = "Console to smoll EXC";
                Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
                Console.Write(s);
                Console.SetCursorPosition((Console.WindowWidth - 4), Console.CursorTop);
                Console.WriteLine("██");
            }
        }
    }
}
