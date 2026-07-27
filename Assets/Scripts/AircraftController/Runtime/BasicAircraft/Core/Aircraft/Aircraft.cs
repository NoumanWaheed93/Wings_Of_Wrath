using UnityEngine;
using Locomotion;
using Utilities;
using Common;

namespace AircraftController
{
    public class Aircraft : IAircraft
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

        public Aircraft(IAircraftMovementData movementData, Transform transform, Rigidbody rigidbody)
        {
            this.transform = transform;
            this.rigidbody = rigidbody;

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

        public void Spawn(Vector3 startPosition, Quaternion startRotation, bool isInAir = false, float startSpeed = 0f)
        {
            if (isInAir)
            {
                stateMachine.ChangeState(stateInAir);
            }
            else
            {
                stateMachine.ChangeState(stateOnGround);
            }

            movementHandler.Initialize(startPosition, startRotation, startSpeed);
        }

        public void Update(float simulationDeltaTime)
        {
            stateMachine.currentState.Update(simulationDeltaTime);
            orientationController.Update(simulationDeltaTime);
        }

        public void FixedUpdate(float simulationDeltaTime)
        {
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
           // Debug.Log($"Seeking speed : {targetSpeed}");
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

    }

}
