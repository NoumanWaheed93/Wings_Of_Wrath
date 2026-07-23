using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController.AircraftAI
{
    public class StateBreakFormation : AIState
    {
        private Vector3 breakDestination;

        public StateBreakFormation(AIStateMachine stateMachine, AircraftAIController aircraftController) : base(stateMachine, aircraftController)
        {
        }

        public override void Enter()
        {
            breakDestination = aircraftController.GetBreakFormationTargetDestination();
        }

        public override void Exit()
        {
        }

        public override void Update(float simulationDeltaTime)
        {
            aircraftController.TurnTowardsPosition(breakDestination);
            if (Vector3.Distance(aircraftController.transform.position, breakDestination) <= GlobalAircraftControllerSettings.wayPointReachedDistance 
                || aircraftController.IsFormationBreaking == false)
            {
                stateMachine.ChangeState(aircraftController.stateFollowWaypoints);
                return;
            }
        }

    }

}
