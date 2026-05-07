using System.Collections;
using System.Collections.Generic;
using AircraftController;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerKeyboardInputManager : IAircraftPlayerInputManager
    {
        public float SteerDirection
        {
            get
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                return horizontalInput;
            }
        }

        public bool IsAfterBurnerOn => Input.GetKey(KeyCode.LeftShift);    
    }

}
