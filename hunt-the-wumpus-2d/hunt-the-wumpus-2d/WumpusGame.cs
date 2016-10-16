﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace hunt_the_wumpus_2d
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class WumpusGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly InputManager _inputManager;
        private Camera2D _camera;
        private SpriteFont _font;
        private Map _map;
        private MessageHandler _messageHandler;
        private SpriteBatch _spriteBatch;
        private TiledMap _tiledMap;
        private readonly bool _isCheatMode;

        public WumpusGame(bool isCheatMode)
        {
            _isCheatMode = isCheatMode;
            _inputManager = InputManager.Instance;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            const int weight = 900;
            const int height = 520;
            _map = new Map(_isCheatMode);
            _camera = new Camera2D(new BoxingViewportAdapter(Window, GraphicsDevice, weight, height));
            _graphics.PreferredBackBufferWidth = weight;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _messageHandler = MessageHandler.Instance(Content);
            _messageHandler.LoadContent();

            _tiledMap = Content.Load<TiledMap>("map");
            _camera.LookAt(new Vector2(_tiledMap.WidthInPixels, _tiledMap.HeightInPixels));
            _font = Content.Load<SpriteFont>("output");
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            _tiledMap.Dispose();
            Content.Dispose();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update();

            if (_inputManager.KeyPressed(Keys.Escape))
                Exit();

            if (_inputManager.KeyPressed(Keys.A))
                _messageHandler.AddMessageToWrite("Super bat attack blah blah blah here you go.");
            if (_inputManager.KeyPressed(Keys.B))
                _messageHandler.AddMessageToWrite("Hi there.");

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _tiledMap.Draw(_spriteBatch, _camera);

            var rooms = _tiledMap.GetObjectGroup("entities").Objects
                .Where(e => e.Type == "room");
            var player = _tiledMap.GetObjectGroup("entities").Objects
                .Single(e => e.Type == "pit");

            int gid = player.Gid ?? default(int);

            var playerTexture = _tiledMap.GetTileRegion(gid);

            var sprite = new Sprite(playerTexture) {Position = new Vector2(player.X, player.Y)};
            _spriteBatch.Draw(sprite);

            foreach (var room in rooms)
            {
                _spriteBatch.DrawString(_font, room.Name, new Vector2(room.X - 25, room.Y - 40), Color.White);
                _spriteBatch.Draw(new Sprite(playerTexture) {Position = new Vector2(room.X, room.Y)});
            }

            _messageHandler.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}