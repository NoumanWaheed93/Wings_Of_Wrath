using UnityEngine;
using Locomotion;
using Common;
using UnityEditor;
using Zenject;
using FormationSystem;
using HealthSystem;
using TargetingSystem;

namespace AircraftController
{
    public class AircraftMonoBehaviour : MonoBehaviour, ISpeedProvider, ITargetable
    {
        private Team team;
        public Team Team { get => team; set => team = value; }
        
        [SerializeField]
        private Sensor[] sensors;

        public float CurrSpeed { get { return aircraft.MovementHandler.CurrSpeed; } }

        private IAircraft aircraft;
        public IAircraft Aircraft { get => aircraft; }

        private IFormationMember formationMember;
        public IFormationMember FormationMember { get => formationMember; }

        private IAircraftController aircraftController;
        public IAircraftController AircraftController { get => aircraftController; }

        public ITransform Transform => formationMember.Transform;

        private Pool pool;

        private Health health;

        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
                Handles.Label(transform.position, CurrSpeed.ToString());
        }

        [Inject]
        public void Init(IAircraft aircraft, IFormationMember formationMember, IAircraftController controller, Team team, Health health, Pool pool)
        {
            this.team = team;
         
            this.aircraft = aircraft;
            this.formationMember = formationMember;
            this.aircraftController = controller;
            this.pool = pool;
            this.health = health;
            health.onHealthDepleted += OnDie;
        }

        public void PrepareToLand(Airstrip airstrip)
        {
            aircraft.AirStripToLandOn = airstrip;
        }

        private void OnDie()
        {
            pool.Despawn(this);
        }

        public class Pool : MemoryPool<AircraftMonoBehaviour>
        {
        }
    }
}
