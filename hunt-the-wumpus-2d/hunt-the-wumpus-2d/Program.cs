using System;

namespace hunt_the_wumpus_2d
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] arguments)
        {
            bool isCheatMode = arguments.Length > 0 && arguments[0].ToLower() == "cheat";

            using (var game = new WumpusGame(isCheatMode))
                game.Run();
        }
    }
}