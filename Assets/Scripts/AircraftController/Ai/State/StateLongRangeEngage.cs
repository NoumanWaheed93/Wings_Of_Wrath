using UnityEngine;

namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Long range engagement: turn to face the target, launch a single guided missile once
    /// roughly aligned, then hand back to the waypoint/formation AI so the aircraft rejoins
    /// its formation. The missile does the homing from here, so this state does not chase.
    /// </summary>
    public class StateLongRangeEngage : AIState
    {
        // How closely the nose must point at the target before we launch, in degrees.
        private const float fireAlignmentThreshold = 20f;
        // Safety valve: if we cannot get aligned within this window, fire anyway and disengage
        // rather than orbiting the target forever.
        private const float maxAimDuration = 8f;

        private readonly Transform target;

        private float aimElapsed;
        private bool hasEngaged;

        public StateLongRangeEngage(AIStateMachine stateMachine, AircraftAIController aircraftController, Transform target)
            : base(stateMachine, aircraftController)
        {
            this.target = target;
        }

        public override void Enter()
        {
            aimElapsed = 0f;
            hasEngaged = false;
            aircraftController.SetThrottleNormal();
        }

        public override void Exit()
        {
        }

        public override void Update(float simulationDeltaTime)
        {
            // Target destroyed or despawned before we could fire, nothing to engage.
            if (target == null || target.gameObject.activeInHierarchy == false)
            {
                RejoinFormation();
                return;
            }

            aircraftController.TurnTowardsPosition(target.position);
            aimElapsed += simulationDeltaTime;

            if (hasEngaged == false && (IsAlignedWithTarget() || aimElapsed >= maxAimDuration))
            {
                // Fire once and disengage regardless of whether ammo/fire-rate allowed the shot,
                // so an out-of-ammo aircraft does not get stuck circling the target.
                aircraftController.FireAt(target);
                hasEngaged = true;
            }

            if (hasEngaged)
            {
                RejoinFormation();
            }
        }

        private bool IsAlignedWithTarget()
        {
            Vector3 toTarget = target.position - aircraftController.transform.position;
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                return true;
            }

            float angle = Vector3.Angle(aircraftController.transform.forward, toTarget);
            return angle <= fireAlignmentThreshold;
        }

        private void RejoinFormation()
        {
            // Returning to the waypoint state mirrors StateBreakFormation: the waypoint state
            // itself hands off to the formation-following state when this aircraft is a member.
            stateMachine.ChangeState(aircraftController.stateFollowWaypoints);
        }
    }
}
