using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class BottomlessPit : DeadlyHazard
    {
        public BottomlessPit(int roomNumber, TextureRegion2D texture, Vector2 position)
            : base(roomNumber, texture, position)
        {
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void PrintLocation()
        {
            Console.WriteLine($"Bottomless pit in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Console.WriteLine(Message.PitWarning);
        }

        public override EndState DetermineEndState(int playerRoomNumber)
        {
            return playerRoomNumber == RoomNumber
                ? new EndState(true, $"{Message.FellInPit}\n{Message.LoseMessage}")
                : new EndState();
        }
    }
}