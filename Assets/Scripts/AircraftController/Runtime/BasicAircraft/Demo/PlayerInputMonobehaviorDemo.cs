using UnityEngine;
using UnityEngine.InputSystem;

namespace AircraftController
{
    public class PlayerInputMonobehaviorDemo : MonoBehaviour, IAircraftPlayerInputManager
    {
        [SerializeField]
        private InputAction horizontalInput;

        [SerializeField]
        private InputAction afterBurnerInput;

        public float SteerDirection
        {
            get; 
            private set;
        } 

        public bool IsAfterBurnerOn 
        { 
            get; 
            private set;
        }

        private AircraftPlayerController playerController;

        private void Start()
        {
            playerController = new AircraftPlayerController(this);
            playerController.Aircraft = GetComponent<AircraftMonoBehaviour>().Aircraft;
            horizontalInput.Enable();
            afterBurnerInput.Enable();
        }

        private void Update()
        {
            SteerDirection = horizontalInput.ReadValue<float>();
            IsAfterBurnerOn = afterBurnerInput.IsPressed();
            playerController.Update(Time.deltaTime);
        }

    }

}
