using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d.Entities
{
    public abstract class Entity
    {
        protected Entity(int roomNumber, TextureRegion2D texture, Vector2 position)
        {
            RoomNumber = roomNumber;
            Texture = texture;
            Position = position;
        }

        public int RoomNumber { get; protected set; }
        public TextureRegion2D Texture { get; }
        public Vector2 Position { get; set; }
        public abstract void Update(GameTime time);

        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(new Sprite(Texture) {Position = Position});
        }

        public abstract void PrintLocation();
    }
}