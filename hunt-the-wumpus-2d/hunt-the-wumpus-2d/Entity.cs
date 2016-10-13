using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d
{
    internal abstract class Entity
    {
        public virtual void LoadContent()
        {
        }

        public abstract void UnloadContent();

        public abstract void Update(GameTime time);

        public abstract void Draw(SpriteBatch batch);
    }
}