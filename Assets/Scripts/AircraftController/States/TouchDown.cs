using UnityEngine;

namespace AircraftController
{
    public class TouchDown : AircraftState
    {
        public TouchDown(AircraftStateMachine stateMachine, Aircraft aircraftController) :
            base(stateMachine, aircraftController)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update(float simulationDeltaTime)
        {
            LowerAltitude();
            aircraftController.SeekSpeed(GlobalAircraftControllerSettings.touchDownSpeed);
            aircraftController.MovementHandler.Turn(aircraftController.TurnInput);
            if (IsAbortIntended())
            {
                Debug.Log("Aborting");
                if (CanAbort())
                {
                    Debug.Log("Do Abort");
                    DoAbort();
                }
                else
                {
                    Debug.Log("Do Crash");
                    DoCrash();
                }
            }
            else if (IsTouchDownDone())
            {
                Debug.Log("Done touch down");
                DoLand();
            }
        }

        private bool IsAbortIntended()
        {
            return aircraftController.HasDeviatedFromLine(aircraftController.RunwayInUse.FinalApproach.position,
                aircraftController.RunwayInUse.TouchDownPoint.position, GlobalAircraftControllerSettings.touchDownAcceptableDeviation);
        }

        private bool CanAbort()
        {
            return true;
        }

        private void DoAbort()
        {
            stateMachine.ChangeState(aircraftController.StateInAir);
            aircraftController.RunwayInUse = null;
        }

        private void DoCrash()
        {
        }

        private bool IsTouchDownDone()
        {
            return (Vector3.Distance(aircraftController.Transform.position,
                aircraftController.RunwayInUse.TouchDownPoint.position) < GlobalAircraftControllerSettings.wayPointReachedDistance);
        }

        private void DoLand()
        {
            stateMachine.ChangeState(aircraftController.StateLanded);
        }

        private void LowerAltitude()
        {
            aircraftController.CalculateAndSetPitch(aircraftController.RunwayInUse.TouchDownPoint.position);
        }
    }
}
