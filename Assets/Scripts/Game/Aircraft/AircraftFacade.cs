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
using CommandSystem;
using Game.Commands;

namespace Game
{
    public class AircraftFacade : MonoBehaviour, ISpeedProvider, ITargetable, ICommandable, ICommandTarget
    {
        private const string LOG_FORMAT = "<color=#FF0000><b>[AircraftFacade]</b></color> {{0}}";

        private IAircraft aircraft;
        public IAircraft Aircraft {get => aircraft;}
        
        public float CurrSpeed { get { return aircraft.MovementHandler.CurrSpeed; } }

        private Team team;
        public Team Team { get => team; set => team = value; }
        
        public Transform Transform => aircraft.Transform;

        private IFormationMember<AircraftFormationMember> formationMember;
        public IFormationMember<AircraftFormationMember> FormationMember { get => formationMember; }

        private AircraftAIController aiController;
        public AircraftAIController AIController { get => aiController; }

        public string Name => name;

        [SerializeField]
        private AircraftMonoBehaviour monoBehaviour;

        private Pool pool;

        private Health health;

        private CommandManager commandManager;
        private CommandTargetManager commandTargetManager;

        [Inject]
        public void Init(IAircraft aircraft, AircraftAIController aiController, AircraftFormationMember formationMember, Team team, Health health,
            CommandManager commandManager, CommandTargetManager commandTargetManager)
        {
            this.team = team;
            this.aircraft = aircraft;
            this.aiController = aiController;
            this.health = health;
            this.commandManager = commandManager;
            this.commandTargetManager = commandTargetManager;

            this.formationMember = formationMember;
            this.formationMember.Self.aircraft = aircraft;
            this.formationMember.Self.aiController = aiController;

            monoBehaviour.Init(aircraft, team);
            health.onHealthDepleted += OnDie;
        }

        public void GiveCommand(ICommand command)
        {
            command.Execute(this);
        }

        public void SetPool(Pool pool)
        {
            this.pool = pool;
        }

        public void Spawn(bool isInAir, float startAltitude, float startSpeed)
        {
            aircraft.Spawn(isInAir, startAltitude, startSpeed);
        }

        private void OnDie()
        {
            Debug.LogFormat(LOG_FORMAT, "OnDie()");
            commandManager.RemoveCommandable(this);
            commandTargetManager.RemoveTarget(this);
            if (formationMember.Formation != null)
            {
                bool wasLeader = formationMember.PositionIndex == 0;
                int currentWaypointIndex = aiController.stateFollowWaypoints.CurrentWaypointIndex;

                formationMember.Formation.RemoveMember(formationMember.Self);

                AircraftFormationMember newLeader = formationMember.Formation.leader;
                if (wasLeader && newLeader != null)
                {
                    newLeader.aiController.stateFollowWaypoints.CurrentWaypointIndex = currentWaypointIndex;
                }
            }
            gameObject.SetActive(false);
            pool.Despawn(this);
        }


        public class Pool : MonoMemoryPool<bool, float, float, AircraftFacade>
        {
            protected override void OnCreated(AircraftFacade aircraft)
            {
                base.OnCreated(aircraft);
                aircraft.SetPool(this);
            }

            protected override void Reinitialize(bool isInAir, float startAltitude, float startSpeed, AircraftFacade aircraft)
            {
                aircraft.Spawn(isInAir, startAltitude, startSpeed);
            }
        }

    }

}
