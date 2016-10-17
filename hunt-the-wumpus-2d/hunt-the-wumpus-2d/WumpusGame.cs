using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace hunt_the_wumpus_2d
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class WumpusGame : Game
    {
        public enum GameState
        {
            Playing,
            GameOver,
            ActionPrompt,
            ShootingArrow,
            PlayerMove,
            PlayAgain,
            SameSetup
        }

        public static GameState State = GameState.Playing;

        private static readonly Logger Log = Logger.Instance;
        private readonly GraphicsDeviceManager _graphics;
        private readonly InputManager _inputManager;
        private readonly bool _isCheatMode;
        private Camera2D _camera;
        private SpriteFont _font;
        private Logger _logger;
        private Map _map;
        private SpriteBatch _spriteBatch;
        private TiledMap _tiledMap;
        private BoxingViewportAdapter _viewportAdapter;

        public WumpusGame(bool isCheatMode)
        {
            _isCheatMode = isCheatMode;
            _inputManager = InputManager.Instance;
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            const int weight = 900;
            const int height = 520;
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, weight, height);
            _camera = new Camera2D(_viewportAdapter);
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.ApplyChanges();

            Log.Write("Press Escape at any point to quit.");

            _inputManager.KeyPressed += (sender, args) =>
            {
                if (args.Key == Keys.Escape)
                    Exit();
            };

            _inputManager.KeyReleased += (sender, args) =>
            {
                if (State == GameState.ActionPrompt && args.Key == Keys.S)
                    _map.PerformCommand("S");
                else if (State == GameState.ActionPrompt && args.Key == Keys.M)
                    _map.PerformCommand("M");
                else if (State == GameState.ActionPrompt && args.Key == Keys.Q)
                    Exit();
            };

            _inputManager.KeyReleased += (sender, args) =>
            {
                if (State == GameState.SameSetup && args.Key == Keys.Y)
                {
                    State = GameState.Playing;
                    _map.Reset();
                }
            };

            _inputManager.KeyReleased += (sender, args) =>
            {
                if (State == GameState.PlayAgain && args.Key == Keys.Y)
                {
                    Log.Write(Message.SetupPrompt);
                    State = GameState.SameSetup;
                }
                else if (State == GameState.PlayAgain && args.Key == Keys.N)
                    Exit();
            };
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("output");
            _logger = Logger.Instance;

            _tiledMap = Content.Load<TiledMap>("map");
            _camera.LookAt(new Vector2(_tiledMap.WidthInPixels, _tiledMap.HeightInPixels));
            _map = new Map(_isCheatMode, _tiledMap, _font);
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
            _inputManager.Update(gameTime);

            if (State == GameState.Playing)
            {
                _map.Update();
                if (State == GameState.Playing)
                {
                    _logger.Write(Message.ActionPrompt);
                    State = GameState.ActionPrompt;
                }
                else if (State == GameState.GameOver)
                {
                    State = GameState.PlayAgain;
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _viewportAdapter.GetScaleMatrix());
            _logger.Messages.ForEach(m => _spriteBatch.DrawString(_font, m.Value, m.Position, m.Color));

            _tiledMap.Draw(_spriteBatch, _camera);
            _map.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}