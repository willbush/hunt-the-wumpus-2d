using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class Wumpus : DeadlyHazard
    {
        private readonly int _initialRoomNumber;
        private static readonly Logger Log = Logger.Instance;

        public Wumpus(int roomNumber, TextureRegion2D texture, Vector2 position)
            : base(roomNumber, texture, position)
        {
            _initialRoomNumber = roomNumber;
        }

        private bool IsAwake { get; set; }

        /// <summary>
        ///     Updates the state of the wumpus.
        /// </summary>
        public void Update(Map map)
        {
            if (!IsAwake && map.Player.RoomNumber == RoomNumber)
            {
                Log.Write(Message.WumpusBump);
                IsAwake = true;
            }
            if (!IsAwake && map.Player.CrookedArrowCount < map.Player.MaxArrows)
                IsAwake = true;

            if (IsAwake)
            {
                if (!TryMove(map) && map.Player.RoomNumber == RoomNumber)
                {
                    Log.Write(Message.WumpusGotYou);
                    Log.Write(Message.LoseMessage);
                    WumpusGame.State = WumpusGame.GameState.GameOver;
                }
            }
        }

        /// <summary>
        ///     Moves the wumpus with a 75% chance.
        /// </summary>
        private bool TryMove(Map map)
        {
            if (!WumpusFeelsLikeMoving()) return false;

            RoomNumber = map.GetSafeRoomNextTo(RoomNumber);
            if (map.IsCheatMode)
                Log.Write($"Wumpus moved to {RoomNumber}");
            return true;
        }

        private static bool WumpusFeelsLikeMoving()
        {
            return new Random().Next(1, 101) > 25; // 75% chance wumpus feels like moving.
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void PrintLocation()
        {
            Log.Write($"Wumpus in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Log.Write(Message.WumpusWarning);
        }

        /// <summary>
        ///     Determine the game end state given the player's current room number.
        /// </summary>
        /// <param name="playerRoomNumber">current player room number</param>
        /// <returns>end state</returns>
        public override EndState DetermineEndState(int playerRoomNumber)
        {
            if (playerRoomNumber == RoomNumber)
                return new EndState(true, $"{Message.WumpusGotYou}\n{Message.LoseMessage}");

            return new EndState();
        }

        /// <summary>
        ///     Resets Wumpus to initial state.
        /// </summary>
        public void Reset()
        {
            RoomNumber = _initialRoomNumber;
            IsAwake = false;
        }
    }
}