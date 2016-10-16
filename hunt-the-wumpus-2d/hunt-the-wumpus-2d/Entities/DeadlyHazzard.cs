using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class DeadlyHazard : Hazard
    {
        protected DeadlyHazard(int roomNumber, TextureRegion2D texture) : base(roomNumber, texture)
        {
        }

        public abstract EndState DetermineEndState(int playerRoomNumber);
    }
}