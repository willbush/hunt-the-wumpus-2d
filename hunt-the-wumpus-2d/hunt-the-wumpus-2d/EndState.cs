namespace hunt_the_wumpus_2d
{
    /// <summary>
    ///     This class basically serves as a immutable tuple that allows me to pass
    ///     the game end state in a functional style without having to introduce mutable fields
    ///     or break control of flow with system exits and other lazy hacks.
    /// </summary>
    public class EndState
    {
        private static readonly Logger Log = Logger.Instance;

        internal EndState()
        {
        }

        internal EndState(bool isGameOver, string gameOverMessage)
        {
            IsGameOver = isGameOver;
            GameOverMessage = gameOverMessage;
        }

        public bool IsGameOver { get; }
        private string GameOverMessage { get; }

        public void Print()
        {
            Log.Write("");
            if (string.IsNullOrEmpty(GameOverMessage)) return;

            Log.Write(GameOverMessage);
            Log.Write("");
        }
    }
}