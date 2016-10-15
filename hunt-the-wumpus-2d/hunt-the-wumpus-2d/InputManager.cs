using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace hunt_the_wumpus_2d
{
    public class InputManager
    {
        private static InputManager _instance;
        private KeyboardState _currentKeyState;
        private KeyboardState _previousKeyState;

        public static InputManager Instance => _instance ?? (_instance = new InputManager());

        public void Update()
        {
            _previousKeyState = _currentKeyState;
            _currentKeyState = Keyboard.GetState();
        }

        /// <summary>
        /// Uses previous and current key state to determine of the given keys were pressed.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>true if the current key state is down and the previous state was up.</returns>
        public bool KeyPressed(params Keys[] keys)
        {
            return keys.Any(key => _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key));
        }

        /// <summary>
        /// Uses previous and current key state to determine of the given keys were released.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>true if the current key state is up and the previous state was down.</returns>
        public bool KeyReleased(params Keys[] keys)
        {
            return keys.Any(key => _currentKeyState.IsKeyUp(key) && _previousKeyState.IsKeyDown(key));
        }
    }
}