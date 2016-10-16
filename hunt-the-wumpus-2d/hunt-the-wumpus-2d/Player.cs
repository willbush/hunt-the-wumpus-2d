using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.TextureAtlases;

namespace hunt_the_wumpus_2d
{
    class Player : ISpriteBatchDrawable
    {
        public bool IsVisible { get; }
        public TextureRegion2D TextureRegion { get; }
        public Vector2 Position { get; }
        public float Rotation { get; }
        public Vector2 Scale { get; }
        public Color Color { get; }
        public Vector2 Origin { get; }
        public SpriteEffects Effect { get; }
    }
}
