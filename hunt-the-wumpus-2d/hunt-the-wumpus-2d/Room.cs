using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d
{
    internal class Room : Entity
    {
        public const int SideLen = 50;
        public readonly Color Color;
        public readonly Rectangle Rectangle;
        private SpriteFont _font;

        public Room(int roomNumber, Vector2 position, Texture2D texture, ContentManager content,
            HashSet<int> adjacentRooms)
        {
            Position = position;
            RoomNumber = roomNumber;
            Texture = texture;
            Content = content;
            AdjacentRooms = adjacentRooms;
            Rectangle = new Rectangle((int) position.X, (int) position.Y, SideLen, SideLen);
            Color = Color.BlanchedAlmond;
        }

        public Texture2D Texture { get; }

        public ContentManager Content { get; }
        public Vector2 Position { get; }
        public int RoomNumber { get; }

        public HashSet<int> AdjacentRooms { get; }

        public override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("output");
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Rectangle, Color);
            batch.DrawString(_font, RoomNumber.ToString(), Position, Color.Black);
        }

        internal class Builder
        {
            public Texture2D Texture { get; set; }
            public ContentManager Content { get; set; }
            public Vector2 Position { get; set; }
            public int RoomNumber { get; set; }
            public HashSet<int> AdjacentRooms { get; set; }

            public Room Build()
            {
                return new Room(RoomNumber, Position, Texture, Content, AdjacentRooms);
            }
        }
    }
}