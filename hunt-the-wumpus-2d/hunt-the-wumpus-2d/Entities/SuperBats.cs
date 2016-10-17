using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class SuperBats : Hazard
    {
        private static readonly Logger Log = Logger.Instance;

        public SuperBats(int roomNumber, TextureRegion2D texture, Vector2 position)
            : base(roomNumber, texture, position)
        {
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void PrintLocation()
        {
            Log.Write($"SuperBats in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Log.Write(Message.BatWarning);
        }

        /// <summary>
        ///     Moves player to a random location on the map if they enter the
        ///     same room as a super bat.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>true if the bat snatched the player into another room</returns>
        public bool TrySnatch(Player player)
        {
            if (player.RoomNumber != RoomNumber) return false;

            Log.Write(Message.BatSnatch);
            player.Move(Map.GetAnyRandomRoomNumber());
            return true;
        }
    }
}