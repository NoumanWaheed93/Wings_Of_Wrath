using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AircraftController.AircraftAI
{
    public class StateLanding : AIState
    {
        public enum LandingPhase { 
            InitialApproachPrep,
            InitialApproach,
            FinalApproach,
            TouchDown
        }

        private LandingPhase phase;
        public LandingPhase Phase => phase;

        private float reachDistance = 60; //If the distance to the targetPosition is less than reachDistance, it means the target is reached.

        private int holdingPatternIndex = 0; // 0 means, aircraft has not started holding yet. 

        public StateLanding(AIStateMachine stateMachine, AircraftAIController aircraftController) : base(stateMachine, aircraftController)
        {
        }

        public override void Enter()
        {
            phase = LandingPhase.InitialApproachPrep;

            //aircraftController.aircraft.MovementHandler.SetBrake(0);
            //aircraftController.aircraft.AfterBurnerInput = false;
        }

        public override void Exit()
        {

        }

        public override void Update(float simulationDeltaTime)
        {
            Runway runway = aircraftController.aircraft.RunwayInUse;
            if(runway != null)
            {
                switch (phase)
                {
                    case LandingPhase.InitialApproachPrep:
                        Debug.Log("InitialApproachPrep");
                        InitialApproachPreparation();
                        break;
                    case LandingPhase.InitialApproach:
                        Debug.Log("Initial approach");
                        InitialApproach();
                        break;
                    case LandingPhase.FinalApproach:
                        Debug.Log("Final Approach");
                        FinalApproach();
                        break;
                    case LandingPhase.TouchDown:
                        Debug.Log("Touch down");
                        TouchDown();
                        break;
                }
            }
            else
            {
            //    Debug.Log("Returning home");
                ReturningHome();
            }
        }

        private void ReturningHome()
        {  
            // 1. Go to the home base
            Runway homeRunway = aircraftController.aircraft.HomeRunway;

            Vector3 RunwayDirection = homeRunway.TouchDownPoint.position - homeRunway.FinalApproach.position;
            Vector3 Waypoint1 = homeRunway.InitialApproach.position - RunwayDirection;
           
            if(holdingPatternIndex == 2)
            {
                Waypoint1 -= new Vector3(0, 0, 1000);
            }

            if(holdingPatternIndex != 0) //The aircraft is holding 
            {
                if (homeRunway.IsInUse == false)
                {
                    phase = LandingPhase.InitialApproachPrep;
                    homeRunway.IsInUse = true;
                    aircraftController.aircraft.RunwayInUse = homeRunway;
                }
            }

        //    Debug.Log($"Holding pattern index is {holdingPatternIndex}");
            // 2. When near the home base. If it is not in use, go to the initial approach.
            if (GoToPosition(Waypoint1))
            {
                if(holdingPatternIndex == 0) //Because, 0 and 1 are the same waypoints. They just indicate the state
                {
                    holdingPatternIndex = 1;
                }

                holdingPatternIndex ++;
                if(holdingPatternIndex > 2)
                {
                    holdingPatternIndex = 1;
                }
            }
        }

        private void InitialApproachPreparation()
        {
            Runway homeRunway = aircraftController.aircraft.RunwayInUse;

            Vector3 ApproachDirection = homeRunway.FinalApproach.position - homeRunway.InitialApproach.position;
            Vector3 initalApproachPreparePoint = homeRunway.InitialApproach.position - ApproachDirection.normalized * 200;
            
            if(GoToPosition(initalApproachPreparePoint))
            {
                phase = LandingPhase.InitialApproach;
            }
        }

        private void InitialApproach()
        {
            Runway runway = aircraftController.aircraft.RunwayInUse;
            
            if(GoToPosition(runway.InitialApproach.position))
            {
                phase = LandingPhase.FinalApproach;
            }
        }

        private void FinalApproach()
        {
            Runway runway = aircraftController.aircraft.RunwayInUse;
            if(GoToPosition(runway.FinalApproach.position))
            {
                phase = LandingPhase.TouchDown;
            }
        }

        private void TouchDown()
        {
            Runway runway = aircraftController.aircraft.RunwayInUse;
            GoToPosition(runway.TouchDownPoint.position);
        }

        //Returns true, if the position is reached
        private bool GoToPosition(Vector3 position)
        {
            aircraftController.TurnTowardsPosition(position);
            return Vector3.Distance(position, aircraftController.transform.position) < reachDistance; 
        }

    }

}
