using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d
{
    internal class MessageHandler : Entity
    {
        private const int XPosition = 800;
        private const int YPosition = 500;
        private readonly ContentManager _content;
        private readonly List<Message> _messages;
        private SpriteFont _font;

        public MessageHandler(ContentManager content)
        {
            _content = content;
            _messages = new List<Message>();
        }

        public void AddMessageToWrite(string message, Color color = default(Color))
        {
            if (color == default(Color))
                color = Color.White;

            if (_messages.Count >= 20)
                _messages.RemoveAt(0);

            _messages.ForEach(m => m.Position.Y -= 20);

            _messages.Add(new Message(message, new Vector2(XPosition, YPosition), color));
        }

        public override void LoadContent()
        {
            _font = _content.Load<SpriteFont>("output");
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime time)
        {
        }

        public override void Draw(SpriteBatch batch)
        {
            _messages.ForEach(m => batch.DrawString(_font, m.Value, m.Position, m.Color));
        }

        private class Message
        {
            public readonly Color Color;
            public readonly string Value;
            public Vector2 Position;

            internal Message(string value, Vector2 position, Color color)
            {
                Value = value;
                Position = position;
                Color = color;
            }
        }
    }
}