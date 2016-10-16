using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class DeadlyHazard : Hazard
    {
        protected DeadlyHazard(int roomNumber, TextureRegion2D texture, Vector2 position)
            : base(roomNumber, texture, position)
        {
        }

        public abstract EndState DetermineEndState(int playerRoomNumber);
    }
}