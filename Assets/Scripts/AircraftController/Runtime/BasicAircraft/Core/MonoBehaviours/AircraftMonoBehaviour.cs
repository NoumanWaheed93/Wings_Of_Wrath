using UnityEngine;
using Common;
using Locomotion;

namespace AircraftController
{
    public class AircraftMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        private AircraftMovementData movementData;

        [SerializeField]
        private Team team;
        public Team Team { get => team; set => team = value; }
        
        private IAircraft aircraft;
        public IAircraft Aircraft { get => aircraft; }

        [SerializeField]
        private bool isHandledByMonobehavior = true; //False if it is to be injected from somewhere else
        [SerializeField]
        private Rigidbody rigidbody;

        public float CurrSpeed => aircraft.MovementHandler.CurrSpeed;

        private void Awake()
        {
            if (isHandledByMonobehavior)
            {
                Init(new Aircraft(movementData, transform, rigidbody), team);
                aircraft.Spawn(transform.position, transform.rotation, false, 0);
            }
        }

        private void Update()
        {
            if (isHandledByMonobehavior)
            {
                aircraft.Update(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (isHandledByMonobehavior)
            {
                aircraft.FixedUpdate(Time.fixedDeltaTime);
            }
        }

        public void Init(IAircraft aircraft, Team team)
        {
            this.team = team;
         
            this.aircraft = aircraft;
        }

        public void PrepareToLand(Runway airstrip)
        {
            aircraft.RunwayInUse = airstrip;
        }
    
    }

}
