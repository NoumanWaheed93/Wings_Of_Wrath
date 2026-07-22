using Common;
using FormationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Zenject;

namespace AircraftController.AircraftAI
{
    public class AircraftAIController : IAircraftController, ITickable
    {

        public IAircraft aircraft { get; private set; }
        public Transform transform { get; private set; }

        private AIStateMachine stateMachine = new AIStateMachine();
        public AIStateMachine StateMachine { get => stateMachine; }

        private float altitudeOffset;
        public float AltitudeOffset { get => altitudeOffset; }

        
        public AircraftFormationMember FormationMember { get; private set; }
        public StateFollowWaypoints stateFollowWaypoints { get; private set; }
        public StateFollowFormation stateFollowFormation { get; private set; }
        public StateLanding stateLanding { get; private set; }

        public StateBreakFormation stateBreakFormation { get; private set; }
        public StateLongRangeEngage stateLongRangeEngage { get; private set; }

        private readonly IWeaponController weaponController;

        public bool IsAfterBurnerOn => true;

        public bool IsFormationBreaking { get; set; }

        private float turnInput;
        private float desiredSpeed;

        //The AI Controller can be deactivated if a player is controlling the aircraft now.
        //The deactivated AI controller will just stop giving the input to the aircraft.
        //All other functions would be working, so that AI stays aware of the situation the aircraft is in.
        //And when the player stops controlling the aircraft, the AI controller can make good decisions.
        public bool IsActive { get; set; }

        public AircraftAIController(IAircraft aircraft, AircraftFormationMember formationMember, Transform transform,
            [InjectOptional] IWeaponController weaponController)
        {
            this.aircraft = aircraft;
            this.FormationMember = formationMember;
            this.transform = transform;
            this.weaponController = weaponController;
            stateFollowWaypoints = new StateFollowWaypoints(stateMachine, this, new Vector3[0]); // Initialize with empty waypoints
            stateFollowFormation = new StateFollowFormation(stateMachine, this);
            stateLanding = new StateLanding(stateMachine, this);
            stateBreakFormation = new StateBreakFormation(stateMachine, this);
            stateMachine.Initialize(stateFollowWaypoints);
            IsActive = true;
        }

        public void SetWaypoints(Vector3[] waypoints)
        {
            StateFollowWaypoints newWaypointState = new StateFollowWaypoints(stateMachine, this, waypoints);
            if (stateMachine.currentState == stateFollowWaypoints)
            {
                stateMachine.Initialize(newWaypointState);
            }
            stateFollowWaypoints = newWaypointState;
        }

        /// <summary>
        /// Enters the long range engagement state against the given target: the aircraft aims and
        /// fires a missile, then rejoins its formation.
        /// </summary>
        public void EngageTarget(Transform target)
        {
            stateLongRangeEngage = new StateLongRangeEngage(stateMachine, this, target);
            stateMachine.ChangeState(stateLongRangeEngage);
        }

        /// <summary>
        /// Fires the aircraft's weapon at the target. No-op (returns false) if this aircraft has
        /// no weapon.
        /// </summary>
        public bool FireAt(Transform target)
        {
            if (weaponController == null)
            {
                return false;
            }
            return weaponController.FireAt(target);
        }

        public void Tick()
        {
            Update(Time.deltaTime);
        }

        public void Update(float simulationDeltaTime)
        {
         //   Debug.Log("AI Controller Update");

            stateMachine.currentState.Update(simulationDeltaTime);
            aircraft.AltitudeOffset = altitudeOffset;
            if (IsActive == false)
            {
                return;
            }

        //    Debug.Log("AI Controller giving input to the aircraft");
            aircraft.DesiredSpeed = desiredSpeed;
            aircraft.TurnInput = turnInput;
            aircraft.AfterBurnerInput = IsAfterBurnerOn;
        }

        public float GetDesiredSpeed()
        {
            return desiredSpeed;
        }

        public float GetTurn()
        {
            return turnInput;
        }

        public void SetThrottleNormal()
        {
            desiredSpeed = aircraft.MovementHandler.AerodynamicMovementData.normalAirSpeed;
        }

        public void TurnTowardsPosition(Vector3 targetPosition)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.cyan);
            //Get relative position
            Vector3 relative = transform.InverseTransformPoint(targetPosition);
            turnInput = Mathf.Atan2(relative.x, relative.z) / (Mathf.PI * 0.5f);
        }

        public void FollowFormation()
        {
            AircraftFormationMember leader = FormationMember.Formation.leader;
            AircraftFormationMember myFormationMember = FormationMember.Self;

            Vector3 myPositionInTheFormation = myFormationMember.Formation.GetMemberPositionSpaced(myFormationMember.PositionIndex);
            altitudeOffset = myPositionInTheFormation.y;
            //Get global position
            Vector3 targetPosition = leader.aircraft.Transform.TransformPoint(myPositionInTheFormation);
            desiredSpeed = aircraft.GetSpeedToFollow(targetPosition, leader.aircraft);

            float predictionTime = CalculatePredictionTime(leader.aircraft, FormationMember, myPositionInTheFormation);

            Vector3 leaderPredictedPosition = PredictPosition(leader.aircraft.Transform, leader.aircraft.Velocity.magnitude, leader.aircraft.AngularVelocity.y * Mathf.Rad2Deg, predictionTime);

            targetPosition += leaderPredictedPosition;
            TurnTowardsPosition(targetPosition);
        }

        /// <summary>
        /// Gets the target destination for breaking formation.
        /// </summary>
        public Vector3 GetBreakFormationTargetDestination()
        {

            AircraftFormationMember leader = FormationMember.Formation.leader;
            AircraftFormationMember myFormationMember = FormationMember.Self;

            Vector3 myPositionInTheFormation = myFormationMember.Formation.GetMemberPosition(myFormationMember.PositionIndex);
            altitudeOffset = myPositionInTheFormation.y;

            Vector3 breakDestination = myPositionInTheFormation;
            if(breakDestination.z <= 0)
            {
                breakDestination.z = 1;
            }

            if(breakDestination.x == 0)
            {
                breakDestination.x = 1;
            }

            breakDestination = breakDestination.normalized * 600f;

            //Get global position
            breakDestination = leader.aircraft.Transform.TransformPoint(breakDestination);
            return breakDestination;
        }

        /// <summary>
        /// This is a proof of concept(Jugaar) method to calculate prediction time, for smoother formation following.
        /// </summary>
        /// <param name="leader"></param>
        /// <param name="myFormationMember"></param>
        /// <param name="myPositionInTheFormation"></param>
        /// <returns></returns>
        private float CalculatePredictionTime(IAircraft leader, AircraftFormationMember myFormationMember, Vector3 myPositionInTheFormation)
        {
            float predictionTime = 1;
            float SidewaysDistanceToLeader = Mathf.Abs(myPositionInTheFormation.x) / myFormationMember.Formation.spacing;
            if (SidewaysDistanceToLeader > 1)
            {
                SidewaysDistanceToLeader *= 0.6f;
            }

            if (leader.AngularVelocity.y > 0.1f && myPositionInTheFormation.x < 0)
            {
                predictionTime = 0.5f / SidewaysDistanceToLeader; // If leader is turning right and I am on the left side, predict less
            }
            else if (leader.AngularVelocity.y < -0.1f && myPositionInTheFormation.x > 0)
            {
                predictionTime = 0.5f / SidewaysDistanceToLeader; // If leader is turning left and I am on the right side, predict less
            }
            else if (leader.AngularVelocity.y > 0.1f && myPositionInTheFormation.x > 0)
            {
                predictionTime = 1.5f * SidewaysDistanceToLeader; // If leader is turning right and I am on the right side, predict more
            }
            else if (leader.AngularVelocity.y < -0.1f && myPositionInTheFormation.x < 0)
            {
                predictionTime = 1.5f * SidewaysDistanceToLeader; // If leader is turning left and I am on the left side, predict more
            }

            return predictionTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="forwardSpeed"></param>
        /// <param name="angularSpeedY"></param>
        /// <param name="predictionTime">How far in the future to predict</param>
        /// <returns></returns>
        Vector3 PredictPosition(Transform transform, float forwardSpeed, float angularSpeedY, float predictionTime)
        {
            Vector3 currentPosition = Vector3.zero;

            float totalDistance = forwardSpeed * predictionTime;
            float totalRotation = angularSpeedY * predictionTime;

            // Handle near-zero angular velocity (straight line)
            if (Mathf.Abs(totalRotation) < 0.01f)
            {
                return transform.forward * totalDistance;
            }

            // Calculate circular arc parameters
            float radiusOfTurn = totalDistance / (totalRotation * Mathf.Deg2Rad);

            // The center of the circular arc is perpendicular to the current forward direction
            Vector3 centerOffset = transform.right * radiusOfTurn;
            Vector3 arcCenter = transform.position + centerOffset;

            // Start position on the arc (current position)
            Vector3 startOnArc = transform.position;

            // Calculate end position by rotating around the arc center
            Quaternion rotationAroundCenter = Quaternion.AngleAxis(totalRotation, Vector3.up);
            Vector3 endOnArc = arcCenter + rotationAroundCenter * (startOnArc - arcCenter);

            // Return the predicted offset from current position
            return endOnArc - transform.position;
        }


        /// <summary>
        /// Not using this method for now. Because, it is only creating more chaos
        /// </summary>
        /// <param name="myFormationMember"></param>
        /// <returns></returns>
        private Vector3 CalculateSeparationForce(AircraftFormationMember myFormationMember)
        {
            float separationDistance = myFormationMember.Formation.spacing; // Desired separation distance
            float separationStrength = 1.0f; // Strength of the separation force

            Vector3 separationForce = Vector3.zero;

            foreach (IFormationMember<AircraftFormationMember> member in myFormationMember.Formation.Members)
            {
                if (member != myFormationMember)
                {
                    Vector3 toMember = myFormationMember.aircraft.Transform.position - member.Self.aircraft.Transform.position;
                    float distance = toMember.magnitude;

                    if (distance < separationDistance)
                    {
                        // Calculate the repulsive force inversely proportional to the distance
                        Vector3 repulsiveForce = toMember.normalized / distance;
                        separationForce += repulsiveForce;
                    }
                }
            }

            // Scale the separation force by the desired strength
            separationForce *= separationStrength;

            return separationForce;
        }

    }

}
