using Locomotion;
using UnityEngine;

namespace AircraftController
{
    public interface IAircraft
    {
        bool AfterBurnerInput { get; set; }
        Runway HomeRunway { get; set; }
        Runway RunwayInUse { get; set; }
        AircraftMovementHandler MovementHandler { get; }
        AircraftOrientationController OrientationController { get; }
        FinalApproach StateFinalApproach { get; }
        InAir StateInAir { get; }
        Landed StateLanded { get; }
        AircraftStateMachine StateMachine { get; }
        OnGround StateOnGround { get; }
        TakeOff StateTakeOff { get; }
        TouchDown StateTouchDown { get; }
        float Throttle { get; set; }
        Transform Transform { get; }
        float TurnInput { get; set; }
        float DesiredSpeed { get; set; }
        float AltitudeOffset { get; set; }


        public Vector3 Velocity { get; }
 
        public Vector3 AngularVelocity { get; }
 
        public void Spawn(bool isInAir = false, float startAltitude = 0f, float startSpeed = 0f);
        void Update(float simulationDeltaTime);
        void FixedUpdate(float simulationDeltaTime);
        void CalculateAndSetPitch(float targetAltitude, float targetDistance);
        void CalculateAndSetPitch(Vector3 targetPosition);
        bool HasDeviatedFromLine(Vector3 lineStart, Vector3 lineEnd, float acceptableDeviation);
        void SeekSpeed(float targetSpeed);
        float GetSpeedToFollow(Vector3 targetPosition, IAircraft toFollow);
        float GetRequiredThrottleForSpeed(float targetSpeed);
    }
}