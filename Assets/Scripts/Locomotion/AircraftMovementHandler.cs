using UnityEngine;
using Utilities;

namespace Locomotion
{
    public class AircraftMovementHandler : MovementHandler, IPitchYaw
    {
        private IAircraftMovementData aerodynamicMovementData;
        public IAircraftMovementData AerodynamicMovementData { get => aerodynamicMovementData; }

        private TargetValueSeeker pitchSeeker;
        private TargetValueSeeker rollSeeker;
        private TargetValueSeeker turnSeeker;

        public float RollFactor 
        { 
            get
            {
                return rollSeeker.CurrValue;
            } 
        }

        public float PitchFactor 
        {
            get 
            {
                return transform.forward.y;
            }
        }

        public float Altitude { get { return transform.position.y; } }

        public AircraftMovementHandler(IAircraftMovementData aerodynamicMovementData, Transform transform, Rigidbody rigidbody):base(aerodynamicMovementData, transform, rigidbody)
        {
            this.aerodynamicMovementData = aerodynamicMovementData;
            pitchSeeker = new TargetValueSeeker(aerodynamicMovementData.pitchSpeed);
            rollSeeker = new TargetValueSeeker(aerodynamicMovementData.rollSpeed);
            turnSeeker = new TargetValueSeeker(aerodynamicMovementData.rollSpeed);
        }

        public void Initialize(float startSpeed, float startAltitude)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = startAltitude;
            transform.position = newPosition;
            rigidbody.position = newPosition;
            rigidbody.rotation = transform.rotation;
            rigidbody.velocity = Vector3.zero;
            currSpeed = startSpeed;
        }

        public void SetPitch(float pitch)
        {
            pitchSeeker.Target = Mathf.Clamp(pitch, -1, 1);
        }

        protected override void HandleMovement(float simulationDeltaTime)
        {
            turnSeeker.Target = currTurn;
            rollSeeker.Target = currTurn;
            turnSeeker.Seek(simulationDeltaTime);
            rollSeeker.Seek(simulationDeltaTime);
            pitchSeeker.Seek(simulationDeltaTime);
            HandleCurrSpeed(simulationDeltaTime);

            rigidbody.velocity = transform.forward * currSpeed;

            Vector3 pitchVelocity = transform.right * pitchSeeker.CurrValue * aerodynamicMovementData.maxPitch;
            float turnFactor = 0;


            //--- Done for the better Control feeling ---

            if (Mathf.Sign(turnSeeker.CurrValue) != Mathf.Sign(rollSeeker.CurrValue))
            {
                turnSeeker.Target = currTurn * 0.5f;
            }

            if (Mathf.Sign(turnSeeker.CurrValue) == Mathf.Sign(currTurn))
            {
                turnFactor = Mathf.Min(Mathf.Abs(turnSeeker.CurrValue), Mathf.Abs(currTurn));
                turnFactor *= Mathf.Sign(currTurn);
            }
            else if(currTurn != 0)
            {
                turnSeeker.Target = 0;
                turnSeeker.Seek(10000); //Snap to the target;
                turnSeeker.Target = currTurn;
                turnSeeker.Seek(simulationDeltaTime);
                turnFactor = turnSeeker.CurrValue;
            }

            // --- Above is done for the better control feeling ---

            Vector3 turnVelocity = Vector3.up * turnFactor * aerodynamicMovementData.maxTurn;
            rigidbody.angularVelocity = pitchVelocity + turnVelocity;
            CorrectYRotation();
        }

        private void CorrectYRotation()
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
    
    }

}
