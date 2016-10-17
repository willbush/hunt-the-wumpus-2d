using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;

namespace hunt_the_wumpus_2d
{
    public class InputManager
    {
        private static InputManager _instance;
        private bool _isInitial;
        private TimeSpan _lastPressTime;
        private string _typedString = string.Empty;
        private Keys _previousKey;
        private KeyboardState _previousState;
        private HashSet<Action<string>> _actions;

        private InputManager() : this(new KeyboardListenerSettings())
        {
            _actions = new HashSet<Action<string>>();
        }

        public InputManager(KeyboardListenerSettings settings)
        {
            InitialDelay = settings.InitialDelayMilliseconds;
            RepeatDelay = settings.RepeatDelayMilliseconds;
        }

        public static InputManager Instance => _instance ?? (_instance = new InputManager());

        public int InitialDelay { get; }
        public int RepeatDelay { get; }

        public event EventHandler<KeyboardEventArgs> KeyTyped;
        public event EventHandler<KeyboardEventArgs> KeyPressed;
        public event EventHandler<KeyboardEventArgs> KeyReleased;

        public void Update(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            RaisePressedEvents(gameTime, currentState);
            RaiseReleasedEvents(currentState);
            RaiseRepeatEvents(gameTime, currentState);

            _previousState = currentState;
        }

        private void RaisePressedEvents(GameTime gameTime, KeyboardState currentState)
        {
            if (currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt)) return;

            var pressedKeys = Enum.GetValues(typeof(Keys))
                .Cast<Keys>()
                .Where(key => currentState.IsKeyDown(key) && _previousState.IsKeyUp(key));

            foreach (var key in pressedKeys)
            {
                var args = new KeyboardEventArgs(key, currentState);

                KeyPressed?.Invoke(this, args);

                if (args.Character.HasValue)
                    KeyTyped?.Invoke(this, args);

                _previousKey = key;
                _lastPressTime = gameTime.TotalGameTime;
                _isInitial = true;
            }
        }

        private void RaiseReleasedEvents(KeyboardState currentState)
        {
            var releasedKeys = Enum.GetValues(typeof(Keys))
                .Cast<Keys>()
                .Where(key => currentState.IsKeyUp(key) && _previousState.IsKeyDown(key));

            foreach (var key in releasedKeys)
                KeyReleased?.Invoke(this, new KeyboardEventArgs(key, currentState));
        }

        private void RaiseRepeatEvents(GameTime gameTime, KeyboardState currentState)
        {
            double elapsedTime = (gameTime.TotalGameTime - _lastPressTime).TotalMilliseconds;

            if (currentState.IsKeyDown(_previousKey) &&
                ((_isInitial && elapsedTime > InitialDelay) || (!_isInitial && elapsedTime > RepeatDelay)))
            {
                var args = new KeyboardEventArgs(_previousKey, currentState);

                if (args.Character.HasValue)
                    KeyTyped?.Invoke(this, args);

                _lastPressTime = gameTime.TotalGameTime;
                _isInitial = false;
            }
        }

        public void AddTypedActionPrompt(Action<string> action)
        {
            _typedString = string.Empty;

            if (!_actions.Contains(action))
            {
                KeyTyped += OnKeyTyped(action);
                _actions.Add(action);
            }
        }

        private EventHandler<KeyboardEventArgs> OnKeyTyped(Action<string> responseParser)
        {
            return (sender, args) =>
            {
                if (args.Key == Keys.Back && _typedString.Length > 0)
                {
                    _typedString = _typedString.Substring(0, _typedString.Length - 1);
                }
                else if (args.Key == Keys.Enter)
                {
                    responseParser(_typedString);
                    _typedString = string.Empty;
                }
                else
                {
                    _typedString += args.Character?.ToString() ?? "";
                }
            };
        }
    }
}