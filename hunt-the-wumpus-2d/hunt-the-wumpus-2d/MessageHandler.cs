using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace hunt_the_wumpus_2d
{
    internal class MessageHandler
    {
        private const int MessageLimit = 25;
        private const int MessageOffset = 20;
        private const int XPosition = 570;
        private const int YPosition = 500;
        private static MessageHandler _instance;
        private readonly ContentManager _content;
        private readonly List<Message> _messages;
        private SpriteFont _font;

        private MessageHandler(ContentManager content)
        {
            _content = content;
            _messages = new List<Message>();
        }

        public static MessageHandler Instance(ContentManager content)
            => _instance ?? (_instance = new MessageHandler(content));

        public void AddMessageToWrite(string message, Color color = default(Color))
        {
            if (color == default(Color))
                color = Color.White;

            if (_messages.Count >= MessageLimit)
                _messages.RemoveAt(0);

            _messages.ForEach(m => m.Position.Y -= MessageOffset);

            _messages.Add(new Message(message, new Vector2(XPosition, YPosition), color));
        }

        public void LoadContent()
        {
            _font = _content.Load<SpriteFont>("output");
        }

        public void Draw(SpriteBatch batch)
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