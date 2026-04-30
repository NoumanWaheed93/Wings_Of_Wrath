using UnityEngine;
using Locomotion;
using Common;
using Zenject;
using FormationSystem;
using HealthSystem;
using TargetingSystem;
using AircraftController.AircraftAI;

namespace AircraftController
{
    public class AircraftMonoBehaviour : MonoBehaviour, ISpeedProvider, ITargetable
    {
        private const string LOG_FORMAT = "<color=#FF0000><b>[AircraftMonoBehaviour]</b></color> {{0}}";

        private Team team;
        public Team Team { get => team; set => team = value; }
        
        [SerializeField]
        private Sensor[] sensors;

        public float CurrSpeed { get { return aircraft.MovementHandler.CurrSpeed; } }

        private IAircraft aircraft;
        public IAircraft Aircraft { get => aircraft; }

        private IFormationMember formationMember;
        public IFormationMember FormationMember { get => formationMember; }

        private AircraftAIController aiController;
        public AircraftAIController AIController { get => aiController; }

        public Transform Transform => formationMember.Transform;

        private Pool pool;

        private Health health;

        [Inject]
        public void Init(IAircraft aircraft, AircraftAIController aiController, IFormationMember formationMember, Team team, Health health, Pool pool)
        {
            this.team = team;
         
            this.aircraft = aircraft;
            this.aiController = aiController;
            this.formationMember = formationMember;
            this.pool = pool;
            this.health = health;
            health.onHealthDepleted += OnDie;
        }

        public void Spawn(bool isInAir, float startAltitude, float startSpeed)
        {
            aircraft.Spawn(isInAir, startAltitude, startSpeed);
        }

        public void PrepareToLand(Runway airstrip)
        {
            aircraft.RunwayInUse = airstrip;
        }

        private void OnDie()
        {
            Debug.LogFormat(LOG_FORMAT, "OnDie()");
            gameObject.SetActive(false);
            pool.Despawn(this);
        }

        public class Pool : MonoMemoryPool<bool, float, float, AircraftMonoBehaviour>
        {
            protected override void Reinitialize(bool isInAir, float startAltitude, float startSpeed, AircraftMonoBehaviour aircraft)
            {
                aircraft.Spawn(isInAir, startAltitude, startSpeed);
            }
        }
    
    }

}
