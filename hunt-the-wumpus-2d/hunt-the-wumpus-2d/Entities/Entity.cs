using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Entity
    {
        protected Entity(int roomNumber)
        {
            RoomNumber = roomNumber;
        }

        public int RoomNumber { get; protected set; }
        public abstract void Update(GameTime time);
        public abstract void Draw(SpriteBatch batch);
        public abstract void PrintLocation();
    }
}