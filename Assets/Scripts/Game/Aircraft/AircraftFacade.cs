using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Locomotion;
using TargetingSystem;
using Common;
using Zenject;
using AircraftController;
using HealthSystem;
using AircraftController.AircraftAI;
using FormationSystem;

namespace Game
{
    public class AircraftFacade : MonoBehaviour, ISpeedProvider, ITargetable
    {
        private const string LOG_FORMAT = "<color=#FF0000><b>[AircraftFacade]</b></color> {{0}}";

        private IAircraft aircraft;
        public IAircraft Aircraft {get => aircraft;}
        
        public float CurrSpeed { get { return aircraft.MovementHandler.CurrSpeed; } }

        private Team team;
        public Team Team { get => team; set => team = value; }
        
        public Transform Transform => formationMember.Transform;

        private IFormationMember formationMember;
        public IFormationMember FormationMember { get => formationMember; }

        private AircraftAIController aiController;
        public AircraftAIController AIController { get => aiController; }

        [SerializeField]
        private AircraftMonoBehaviour monoBehaviour;

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

            monoBehaviour.Init(aircraft, team);
            health.onHealthDepleted += OnDie;
        }

        public void Spawn(bool isInAir, float startAltitude, float startSpeed)
        {
            aircraft.Spawn(isInAir, startAltitude, startSpeed);
        }

        private void OnDie()
        {
            Debug.LogFormat(LOG_FORMAT, "OnDie()");
            gameObject.SetActive(false);
            pool.Despawn(this);
        }


        public class Pool : MonoMemoryPool<bool, float, float, AircraftFacade>
        {
            protected override void Reinitialize(bool isInAir, float startAltitude, float startSpeed, AircraftFacade aircraft)
            {
                aircraft.Spawn(isInAir, startAltitude, startSpeed);
            }
        }

    }

}
