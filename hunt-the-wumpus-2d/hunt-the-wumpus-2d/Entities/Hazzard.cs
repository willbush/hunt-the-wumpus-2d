using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Hazard : Entity
    {
        protected Hazard(int roomNumber, TextureRegion2D texture) : base(roomNumber, texture)
        {
        }

        public abstract void PrintHazardWarning();
    }
}