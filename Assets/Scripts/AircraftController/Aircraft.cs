using UnityEngine;
using Locomotion;
using Utilities;
using Common;
using FormationSystem;
using Zenject;

namespace AircraftController
{
    public class Aircraft : IAircraft, ITickable, IFixedTickable
    {
        private AircraftStateMachine stateMachine;
        public AircraftStateMachine StateMachine { get => stateMachine; }

        private AircraftOrientationController orientationController;
        public AircraftOrientationController OrientationController { get => orientationController; }

        private AircraftMovementHandler movementHandler;
        public AircraftMovementHandler MovementHandler { get => movementHandler; }

        private Transform transform;
        public Transform Transform { get => transform; }

        private Rigidbody rigidbody; //do not remove even if it is not used. Only remove it in production review.

        private ISensor[] frontalSensors;
        public ISensor[] FrontalSensors { get; private set; }

        #region States
        private OnGround stateOnGround;
        public OnGround StateOnGround { get => stateOnGround; }

        private TakeOff stateTakeOff;
        public TakeOff StateTakeOff { get => stateTakeOff; }

        private InAir stateInAir;
        public InAir StateInAir { get => stateInAir; }

        private FinalApproach stateFinalApproach;
        public FinalApproach StateFinalApproach { get => stateFinalApproach; }

        private TouchDown stateTouchDown;
        public TouchDown StateTouchDown { get => stateTouchDown; }

        private Landed stateLanded;
        public Landed StateLanded { get => stateLanded; }
        #endregion

        public Vector3 Velocity { get => rigidbody.linearVelocity; }
        public Vector3 AngularVelocity { get => rigidbody.angularVelocity; }

        public float TurnInput { get; set; }
        public float DesiredSpeed { get; set; }
        public float AltitudeOffset { get; set; } = 0f; //This is used to adjust the altitude of the aircraft in formation flying.

        public bool AfterBurnerInput { get; set; }

        private float throttle;
        public float Throttle
        {
            get => throttle;
            set
            {
                throttle = value;
                movementHandler.SetThrottle(value);
            }
        }

        public Runway RunwayInUse { get; set; }

        public Runway HomeRunway { get; set; }

        public IFormationMember<AircraftFormationMember> FormationMember { get; private set; }

        public Aircraft(IAircraftMovementData movementData, Transform transform, Rigidbody rigidbody, IFormationMember<AircraftFormationMember> formationMember)
        {
            this.transform = transform;
            this.rigidbody = rigidbody;
            this.FormationMember = formationMember;

            stateMachine = new AircraftStateMachine();
            movementHandler = new AircraftMovementHandler(movementData, transform, rigidbody);
            orientationController = new AircraftOrientationController(movementData, movementHandler, transform.GetChild(0));

            stateOnGround = new OnGround(stateMachine, this);
            stateTakeOff = new TakeOff(stateMachine, this);
            stateInAir = new InAir(stateMachine, this);
            stateFinalApproach = new FinalApproach(stateMachine, this);
            stateTouchDown = new TouchDown(stateMachine, this);
            stateLanded = new Landed(stateMachine, this);
        }

        public Aircraft SetSensors(ISensor[] sensors)
        {
            this.frontalSensors = sensors;
            return this;
        }

        public void Spawn(bool isInAir = false, float startAltitude = 0f, float startSpeed = 0f)
        {
            if (isInAir)
            {
                stateMachine.ChangeState(stateInAir);
            }
            else
            {
                stateMachine.ChangeState(stateOnGround);
            }

            movementHandler.Initialize(startSpeed, startAltitude);
        }

        public void Update(float simulationDeltaTime)
        {
            stateMachine.currentState.Update(simulationDeltaTime);
            orientationController.Update(simulationDeltaTime);
        }

        public void FixedUpdate(float simulationDeltaTime)
        {
//            Debug.Log("Fixed Update in Aircraft");
            movementHandler.Update(simulationDeltaTime);
        }

        public void CalculateAndSetPitch(float targetAltitude, float targetDistance)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y = targetAltitude;
            Vector3 transformForward = transform.forward;
            transformForward.y = 0;
            transformForward.Normalize();
            targetPosition += transformForward * targetDistance;
            CalculateAndSetPitch(targetPosition);
        }

        public void CalculateAndSetPitch(Vector3 targetPosition)
        {
            Vector3 relative = transform.InverseTransformPoint(targetPosition);
            float targetPitch = -Mathf.Atan2(relative.y, relative.z);
            movementHandler.SetPitch(targetPitch);
        }

        public bool HasDeviatedFromLine(Vector3 lineStart, Vector3 lineEnd, float acceptableDeviation)
        {
            Vector3 planePosition = transform.position;
            Vector3 pointOnApproachLine = Vector3Extensions.FindNearestPointOnLine(lineStart,
                lineEnd, planePosition);
            if (Vector3.Distance(planePosition, pointOnApproachLine) > acceptableDeviation)
                return true;
            return false;
        }

        public void SeekSpeed(float targetSpeed)
        {
            //calculate required throttle
            Debug.Log($"Seeking speed : {targetSpeed}");
            float requiredThrottle = GetRequiredThrottleForSpeed(targetSpeed);
            movementHandler.SetThrottle(requiredThrottle);
        }

        public float GetRequiredThrottleForSpeed(float targetSpeed)
        {
            return targetSpeed / movementHandler.AerodynamicMovementData.maxSpeed;
        }

        //Remove if the break is already handled elsewhere
        private float CalculateRequiredBrakePressure(float targetSpeed)
        {
            float speedDifference = movementHandler.CurrSpeed - targetSpeed;
            float clampedSpeedDifference = Mathf.Clamp(speedDifference, 10, 30);
            float requiredBrakePressure = clampedSpeedDifference / 20f;
            return requiredBrakePressure;
        }

        public float GetSpeedToFollow(Vector3 targetPosition, IAircraft toFollow)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.white);
         
            IFormationMember<AircraftFormationMember> myFormationMember = FormationMember;
            float forwardDistanceToTargetPos = GetDistanceAhead(targetPosition);
            float leaderSpeed = toFollow.Velocity.magnitude;

            Vector3 relativeVelocity = this.Velocity - toFollow.Velocity;
            float closureSpeed = RelativeVelocityUtility.CalculateClosureSpeed(toFollow.Transform.position, Transform.position, relativeVelocity);// CalculateClosureSpeed(leader.Transform.position, myFormationMember.Transform.position, relativeVelocity);

            float throttleRequiredForTargetSpeed = GetRequiredThrottleForSpeed(leaderSpeed);

            float decelerationAtTargetSpeed = Mathf.Lerp(MovementHandler.AerodynamicMovementData.maxDeceleration, 0, throttleRequiredForTargetSpeed);
            float distanceThatCanBeCoveredUntilZeroRelSpeed = RelativeVelocityUtility.GetDistanceToReachSpeed(closureSpeed, 0, -decelerationAtTargetSpeed);

            //Guzara if statement below, with guzara jugaar
            if (forwardDistanceToTargetPos < -1f)
            {
                MovementHandler.SetBrake(0.5f); //Airbrake -> 0.5f, Wheel brake -> 1f
                return movementHandler.AerodynamicMovementData.lowAirSpeed;
            }

            /* If currSpeed is higher than the target speed and the aircraft can reach
             the target position by normal deceleration
             {Decelerate} */
            if (MovementHandler.CurrSpeed > leaderSpeed &&
                distanceThatCanBeCoveredUntilZeroRelSpeed > forwardDistanceToTargetPos - 0.1f)
            {
                //--To Do : Set lower desired speed when the zero rel speed distance is too big --//

                if (distanceThatCanBeCoveredUntilZeroRelSpeed - forwardDistanceToTargetPos > 3)
                {
                    //hit the brakes
                    MovementHandler.SetBrake(0.5f); //Airbrake -> 0.5f, Wheel brake -> 1f
                }
                else
                {
                    MovementHandler.SetBrake(0);
                }
                return leaderSpeed;
            }
            else
            {
                MovementHandler.SetBrake(0);
                return movementHandler.AerodynamicMovementData.maxSpeed;
            }
            //	desiredSpeed += Random.Range(-0.5f, 0.5f);
        }

        /// <summary>
        /// This method returns true if there is an obstacle/ object
        /// right in front.
        /// </summary>
        /// <returns></returns>
        public bool IsCollisionHazardAhead()
        {
            for(int i = 0; i< frontalSensors.Length; i++)
            {
                if (frontalSensors[i].HasSomethingInFront)
                {
                    return true;
                }
            }
            return false;
        }

        private float GetDistanceAhead(Vector3 targetPosition)
        {
            Vector3 ToPosition = targetPosition - transform.position;

            float distanceAhead = Vector3.Dot(ToPosition, transform.forward);
            return distanceAhead;
        }

        //Vector3 IRelativePositionProvider.GetRelativePosition(Vector3 point)
        //{
        //    return transform.InverseTransformPoint(point);
        //}

        //Vector3 IRelativePositionProvider.GetGlobalPosition(Vector3 localPosition)
        //{
        //    return transform.TransformPoint(localPosition);
        //}

        public void Tick()
        {
            Update(Time.deltaTime);
        }

        public void FixedTick()
        {
//            Debug.Log("Fixed tick in Aircraft");
            FixedUpdate(Time.fixedDeltaTime);
        }
    }
}
