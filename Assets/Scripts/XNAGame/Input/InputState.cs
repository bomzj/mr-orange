#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Assets.Scripts.XNAEmulator.Input;
using UnityEngine;
using Assets.Scripts.XNAGame;

namespace GameStateManagement
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        // Mouse state
        public MouseState CurrentMouseState { get; private set; }
        public MouseState LastMouseState { get; private set; }

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
        }

        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update()
        {
            var viewPort = VirtualViewport.Current;

            // Get mouse state
            LastMouseState = CurrentMouseState;
            float invertedScale = 1f/ viewPort.Scale;
            CurrentMouseState = new MouseState
            {
                X = (Input.mousePosition.x - viewPort.X) * invertedScale,
                Y = ((viewPort.Y + viewPort.Height) - Input.mousePosition.y) * invertedScale,
                LeftButton = Input.GetMouseButton(0) ? ButtonState.Pressed : ButtonState.Released
            };
        }

        public bool IsMouseLeftButtonDown()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsNewMouseLeftButtonDown()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed 
              && LastMouseState.LeftButton == ButtonState.Released;
        }
    }
}
