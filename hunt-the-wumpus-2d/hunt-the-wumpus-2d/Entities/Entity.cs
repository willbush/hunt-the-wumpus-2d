using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Entity
    {
        protected Entity(int roomNumber, TextureRegion2D texture)
        {
            RoomNumber = roomNumber;
            Texture = texture;
        }

        public int RoomNumber { get; protected set; }
        public TextureRegion2D Texture { get; }

        public abstract void Update(GameTime time);
        public abstract void Draw(SpriteBatch batch);
        public abstract void PrintLocation();
    }
}