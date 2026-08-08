using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController.AircraftAI
{
    public class StateFollowWaypoints : AIState
    {
        private int currentIndex = 0;
        private Vector3[] wayPoints;

        public int CurrentWaypointIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }

        public StateFollowWaypoints(AircraftAIController aircraftController, Vector3[] wayPoints) : base(aircraftController)
        {
            this.wayPoints = wayPoints;
        }

        public void SetWaypoints(Vector3[] wayPoints)
        {
            this.wayPoints = wayPoints;
            currentIndex = 0;
        }

        public override void Enter()
        {
            aircraftController.SetThrottleNormal();
        }

        public override void Exit()
        {
        }

        public override void Update(float simulationDeltaTime)
        {
            if (wayPoints != null && wayPoints.Length > 0) // If there are any waypoints
            {
                aircraftController.TurnTowardsPosition(wayPoints[currentIndex]);

                if (Vector3.Distance(aircraftController.transform.position, wayPoints[currentIndex]) <= GlobalAircraftControllerSettings.wayPointReachedDistance)
                {
                    currentIndex++;
                    if (currentIndex >= wayPoints.Length)
                    {
                        currentIndex = 0;
                    }
                }
            }
        }

    }

}
