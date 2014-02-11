using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UnityEngine;

namespace GameStateManagement
{
    public class InputAction
    {
        private readonly UnityEngine.KeyCode[] keys;
        private readonly bool newPressOnly;
        
        private delegate bool KeyPress(Keys key);
   
        public InputAction(KeyCode[] keys, bool newPressOnly)
        {
            this.keys = keys;
            this.newPressOnly = newPressOnly;
        }

        public bool Evaluate(InputState state)
        {
            foreach (var key in keys)
            {
                if (Input.GetKeyUp(key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
