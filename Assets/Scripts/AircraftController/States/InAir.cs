using UnityEngine;

namespace AircraftController
{
    public class InAir : AircraftState
    {
        private float altitudeTarget;
        private float pitchDistance; //The shorter distance will make the aircraft pitch aggressively

        private float collisionAvoidanceTime;

        public InAir(AircraftStateMachine stateMachine, Aircraft aircraftController) : base(stateMachine, aircraftController)
        {

        }

        public override void Enter()
        {
            altitudeTarget = GlobalAircraftControllerSettings.flightAltitude;
            pitchDistance = 100f;
            collisionAvoidanceTime = 0;
            if(aircraftController.RunwayInUse != null)
            {
                aircraftController.RunwayInUse.IsInUse = false;
                aircraftController.RunwayInUse = null;
            } 
        }

        public override void Exit()
        {

        }

        public override void Update(float simulationDeltaTime)
        {
            //Can Fire Weapons
            if (IsInitialApproachDone())
            {
                MoveToFinalApproach();
            }

            //CollisionAvoidance(simulationDeltaTime);
            
            aircraftController.SeekSpeed(aircraftController.DesiredSpeed);
            aircraftController.CalculateAndSetPitch(altitudeTarget + aircraftController.AltitudeOffset, pitchDistance);
            aircraftController.MovementHandler.Turn(aircraftController.TurnInput);
        }

        private void CollisionAvoidance(float simulationDeltaTime)
        {
            //if (collisionAvoidanceTime > 0)
            //{
            //    collisionAvoidanceTime -= simulationDeltaTime;
            //}
            //else if (pitchDistance != 100)
            //{
            //    pitchDistance = 100;
            //    altitudeTarget = GlobalAircraftControllerSettings.flightAltitude;
            //}
            //else if (aircraftController.IsCollisionHazardAhead())
            //{
            //    collisionAvoidanceTime = 0.5f;
            //    pitchDistance = 10f;
            //    altitudeTarget -= 10f;
            //}
        }

        private bool IsInitialApproachDone()
        {
            if(aircraftController.RunwayInUse != null)
            {
                bool isNearInitialApproach = Vector3.Distance(aircraftController.Transform.position, aircraftController.RunwayInUse.InitialApproach.position) < 20;
                return isNearInitialApproach;
            }
            return false;
        }

        private void MoveToFinalApproach()
        {
            stateMachine.ChangeState(aircraftController.StateFinalApproach);
        }
    }
}
