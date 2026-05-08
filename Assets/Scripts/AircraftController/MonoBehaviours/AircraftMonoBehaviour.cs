using UnityEngine;
using Common;
using FormationSystem;
using AircraftController.AircraftAI;
using Locomotion;

namespace AircraftController
{
    public class AircraftMonoBehaviour : MonoBehaviour
    {
        private const string LOG_FORMAT = "<color=#FF0000><b>[AircraftMonoBehaviour]</b></color> {{0}}";

        [SerializeField]
        private AircraftMovementData movementData;

        [SerializeField]
        private Team team;
        public Team Team { get => team; set => team = value; }
        
        [SerializeField]
        private Sensor[] sensors;

        private IAircraft aircraft;
        public IAircraft Aircraft { get => aircraft; }

        [SerializeField]
        private bool isAutoInit = true;
        [SerializeField]
        private new Rigidbody rigidbody;

        public float CurrSpeed => aircraft.MovementHandler.CurrSpeed;

        void Start()
        {
            if (isAutoInit)
            {
                Init(new Aircraft(movementData, transform, rigidbody), team);
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
