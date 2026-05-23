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
            }

            public override void Update(float simulationDeltaTime)
            {
                IAircraft leader = aircraftController.FormationMember.Formation.leader.aircraft;

                bool isLeaderLanding = leader.StateMachine.currentState ==  leader.StateFinalApproach;
                if(isLeaderLanding)
                {
                    stateMachine.ChangeState(aircraftController.stateLanding);
                    return;
                }
                aircraftController.FollowFormation();
            }
        }

    }

}
