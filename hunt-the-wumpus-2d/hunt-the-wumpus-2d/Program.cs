using System;

namespace hunt_the_wumpus_2d
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var game = new WumpusGame())
                game.Run();
        }
    }
}