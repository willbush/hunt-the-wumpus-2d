using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public class BottomlessPit : DeadlyHazard
    {
        private static readonly Logger Log = Logger.Instance;

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
            Log.Write($"Bottomless pit in room {RoomNumber}");
        }

        public override void PrintHazardWarning()
        {
            Log.Write(Message.PitWarning);
        }

        public override EndState DetermineEndState(int playerRoomNumber)
        {
            EndState endState;
            if (playerRoomNumber == RoomNumber)
            {
                IsVisible = true;
                endState = new EndState(true, $"{Message.FellInPit}\n{Message.LoseMessage}");
            }
            else
            {
                endState = new EndState();
            }
            return endState;
        }
    }
}