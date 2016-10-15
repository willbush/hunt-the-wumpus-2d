using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hunt_the_wumpus_2d
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class WumpusGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly InputManager _inputManager;
        private Map _map;
        private MessageHandler _messageHandler;
        private SpriteBatch _spriteBatch;

        public WumpusGame()
        {
            _inputManager = InputManager.Instance;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _map = new Map(Content);
            _messageHandler = new MessageHandler(Content);
            _map.LoadContent(GraphicsDevice);
            _messageHandler.LoadContent();
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            _map.UnloadContent();
            Content.Dispose();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_inputManager.KeyPressed(Keys.Escape))
                Exit();

            if (_inputManager.KeyPressed(Keys.A))
                _messageHandler.AddMessageToWrite("Super bat attack blah blah blah here you go.");
            if (_inputManager.KeyPressed(Keys.B))
                _messageHandler.AddMessageToWrite("Hi there.");

            _map.Update(gameTime);
            _inputManager.Update();
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _map.Draw(_spriteBatch);
            _messageHandler.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}