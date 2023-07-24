using System;

namespace ManagedDoom.Silk
{
    public static class SilkProgram
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(ApplicationInfo.Title);
            Console.ResetColor();

            try
            {
                string quitMessage = null;

                using (var app = new SilkDoom(new CommandLineArgs(args)))
                {
                    app.Run();
                    quitMessage = app.QuitMessage;
                }

                if (quitMessage != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(quitMessage);
                    Console.ResetColor();
                    Console.Write("Press any key to exit.");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
                Console.Write("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
