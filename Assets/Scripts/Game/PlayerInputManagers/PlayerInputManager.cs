using System.Collections;
using System.Collections.Generic;
using AircraftController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInputManager : IAircraftPlayerInputManager
    {
        private InputAction steerAction;
        private InputAction afterBurnerAction;
        private InputAction fireCannonAction;
        private InputAction fireMissileAction;

        public float SteerDirection => steerAction.ReadValue<float>();

        public bool IsAfterBurnerOn => afterBurnerAction.IsPressed();

        public PlayerInputManager(InputAction steerAction, InputAction afterBurnerAction)
        {
            this.steerAction = steerAction;
            this.afterBurnerAction = afterBurnerAction;
            steerAction.Enable();
            afterBurnerAction.Enable();
        }
    
    }
}
