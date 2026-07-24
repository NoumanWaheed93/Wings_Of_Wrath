using UnityEngine;

namespace AircraftController
{
    public class Landed : AircraftState
    {
        public Landed(AircraftStateMachine stateMachine, Aircraft aircraftController) 
            : base(stateMachine, aircraftController)
        {
        }

        public override void Enter()
        {
            Debug.Log("Landed aircraft.");
            aircraftController.SeekSpeed(0);
            aircraftController.MovementHandler.SetBrake(1f);
            //Play the particle effect
            //Play the screeching tyres SFX
        }

        public override void Exit()
        {
        }

        public override void Update(float simulationDeltaTime)
        {
            //Slow down to a halt
            //Automatically turn the aircraft to keep it aligned with the runway
            Debug.Log($"magnitude of vel is {aircraftController.Velocity.magnitude}");
            if(aircraftController.Velocity.magnitude < 0.1f)
            {
                aircraftController.RunwayInUse.IsInUse = false;
                aircraftController.Transform.gameObject.SetActive(false);
            }
        }
    
    }

}