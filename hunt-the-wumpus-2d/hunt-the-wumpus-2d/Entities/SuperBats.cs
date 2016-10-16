using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class SuperBats : Hazard
    {
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
            Console.WriteLine($"SuperBats in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Console.WriteLine(Message.BatWarning);
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

            Console.WriteLine(Message.BatSnatch);
            player.Move(Map.GetAnyRandomRoomNumber());
            return true;
        }
    }
}