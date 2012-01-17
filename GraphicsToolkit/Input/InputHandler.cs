using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraphicsToolkit.Input
{
    public delegate void ControllerStatusChangedEvent(PlayerIndex p);

    /// <summary>
    /// Handles various types of inputs across Xbox and Windows
    /// Windows supports the GamePad, Keyboard, and Mouse
    /// Xbox only supports the GamePad
    /// 
    /// The PadState arrays are indexed by player index
    /// </summary>
    public class InputHandler : GameComponent
    {
        public static GamePadState[] LastPadStates = new GamePadState[4];
        public static GamePadState[] PadStates = new GamePadState[4];
#if WINDOWS
        public static MouseState LastMouseState;
        public static MouseState MouseState;
        public static KeyboardState LastKeyState;
        public static KeyboardState KeyState;
#endif

        public static event ControllerStatusChangedEvent ControllerDisconnected;
        public static event ControllerStatusChangedEvent ControllerConnected;

        public InputHandler(Game g)
            : base(g)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            for (PlayerIndex p = PlayerIndex.One; p <= PlayerIndex.Four; p++)
            {
                LastPadStates[p.GetHashCode()] = GamePad.GetState(p);
            }

#if WINDOWS
            LastMouseState = Mouse.GetState();
            LastKeyState = Keyboard.GetState();
#endif
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (PlayerIndex p = PlayerIndex.One; p <= PlayerIndex.Four; p++)
            {
                int index = p.GetHashCode();
                LastPadStates[index] = PadStates[index];
                PadStates[index] = GamePad.GetState(p);

                if (LastPadStates[index].IsConnected && !PadStates[index].IsConnected)
                {
                    if (ControllerDisconnected != null)
                    {
                        ControllerDisconnected(p);
                    }
                }

                if (!LastPadStates[index].IsConnected && PadStates[index].IsConnected)
                {
                    if (ControllerConnected != null)
                    {
                        ControllerConnected(p);
                    }
                }
            }

#if WINDOWS
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();
            LastKeyState = KeyState;
            KeyState = Keyboard.GetState();
#endif
        }

        public static bool IsButtonPressed(Buttons button, PlayerIndex index)
        {
            return PadStates[index.GetHashCode()].IsButtonDown(button);
        }

        public static bool IsNewButtonPress(Buttons button, PlayerIndex index)
        {
            int i = index.GetHashCode();
            return LastPadStates[i].IsButtonUp(button) && PadStates[i].IsButtonDown(button);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return KeyState.IsKeyDown(key);
        }

        public static bool IsNewKeyPress(Keys key)
        {
            return LastKeyState.IsKeyUp(key) && KeyState.IsKeyDown(key);
        }
    }
}