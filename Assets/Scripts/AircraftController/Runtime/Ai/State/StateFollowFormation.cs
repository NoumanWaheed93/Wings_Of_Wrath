using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController
{
    namespace AircraftAI
    {
        public class StateFollowFormation : AIState
        {

            public StateFollowFormation(AircraftAIController aircraftAIController): base(aircraftAIController)
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
                //Only fly a formation position while there actually is a leader to follow.
                //Leaving the formation is handled by the transitions out of this state.
                AircraftFormationMember formationMember = aircraftController.FormationMember;
                if (formationMember.PositionIndex == 0 || formationMember.Formation == null || formationMember.Formation.leader == null)
                {
                    return;
                }

                aircraftController.FollowFormation();
            }
        }

    }

}
