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
        ///     Uses previous and current key state to determine the given key was released.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if the given key have a current key state is down and the previous state was up.</returns>
        public bool KeyPressed(Keys key)
        {
            return _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
        }

        /// <summary>
        ///     Uses previous and current key state to determine the given key was pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if the given key have a current key state is up and the previous state was down.</returns>
        public bool KeyReleased(Keys key)
        {
            return _currentKeyState.IsKeyUp(key) && _previousKeyState.IsKeyDown(key);
        }
    }
}