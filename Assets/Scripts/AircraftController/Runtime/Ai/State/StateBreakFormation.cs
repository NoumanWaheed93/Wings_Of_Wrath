using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController.AircraftAI
{
    public class StateBreakFormation : AIState
    {
        private Vector3 breakDestination;

        /// <summary>
        /// True when the aircraft has arrived at the position it is breaking away to.
        /// </summary>
        public bool HasReachedBreakDestination =>
            Vector3.Distance(aircraftController.transform.position, breakDestination) <= GlobalAircraftControllerSettings.wayPointReachedDistance;

        public StateBreakFormation(AircraftAIController aircraftController) : base(aircraftController)
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
        }

    }

}
