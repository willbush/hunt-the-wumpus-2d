using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class Wumpus : DeadlyHazard
    {
        private readonly int _initialRoomNumber;

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
                Console.WriteLine(Message.WumpusBump);
                IsAwake = true;
            }
            if (!IsAwake && map.Player.CrookedArrowCount < map.Player.MaxArrows)
                IsAwake = true;

            if (IsAwake)
                Move(map);
        }

        /// <summary>
        ///     Moves the wumpus with a 75% chance.
        /// </summary>
        private void Move(Map map)
        {
            if (!WumpusFeelsLikeMoving()) return;

            RoomNumber = map.GetSafeRoomNextTo(RoomNumber);
            if (map.IsCheatMode)
                Console.WriteLine($"Wumpus moved to {RoomNumber}");
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
            Console.WriteLine($"Wumpus in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Console.WriteLine(Message.WumpusWarning);
        }

        /// <summary>
        ///     Determine the game end state given the player's current room number.
        /// </summary>
        /// <param name="playerRoomNumber">current player room number</param>
        /// <returns>end state</returns>
        public override EndState DetermineEndState(int playerRoomNumber)
        {
            if (IsAwake && playerRoomNumber == RoomNumber)
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