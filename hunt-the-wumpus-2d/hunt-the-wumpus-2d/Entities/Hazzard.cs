using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Hazard : Entity
    {
        protected Hazard(int roomNumber, TextureRegion2D texture, Vector2 position)
            : base(roomNumber, texture, position)
        {
        }

        public abstract void PrintHazardWarning();
    }
}