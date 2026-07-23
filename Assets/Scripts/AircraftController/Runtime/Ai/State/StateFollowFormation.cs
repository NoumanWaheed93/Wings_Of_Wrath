using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController
{
    namespace AircraftAI
    {
        public class StateFollowFormation : AIState
        {

            public StateFollowFormation(AIStateMachine stateMachine, AircraftAIController aircraftAIController): base(stateMachine, aircraftAIController)
            {
            }

            public override void Enter()
            {
            }

            public override void Exit()
            {
                aircraftController.aircraft.MovementHandler.SetBrake(0);
                aircraftController.aircraft.AfterBurnerInput = false;
                aircraftController.SetThrottleNormal();
            }

            public override void Update(float simulationDeltaTime)
            {
                if (aircraftController.FormationMember.PositionIndex == 0)
                {
                    stateMachine.ChangeState(aircraftController.stateFollowWaypoints);
                    return;
                }

                IAircraft leader = aircraftController.FormationMember.Formation.leader.aircraft;

                bool isLeaderLanding = leader.StateMachine.currentState ==  leader.StateFinalApproach;
                if(isLeaderLanding)
                {
                    stateMachine.ChangeState(aircraftController.stateLanding);
                    return;
                }

                if (aircraftController.IsFormationBreaking)
                {
                    stateMachine.ChangeState(aircraftController.stateBreakFormation);
                    return;
                }

                aircraftController.FollowFormation();
            }
        }

    }

}
