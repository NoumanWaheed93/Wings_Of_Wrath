using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormationSystem;
using Zenject;
using AircraftController.AircraftAI;
using AircraftController;
using Common;
using CommandSystem;

namespace Game
{
    public class FormationCreator : MonoBehaviour
    {
        public enum Formation
        {
            Trail,
            Echelon,
            ArrowHead,
            BattleSpread
        }

        [SerializeField]
        private Team team;
        [SerializeField]
        private Formation formationType;


        [SerializeField]
        private int count;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private float spacing;
        [SerializeField]
        private float altitudeSpacing;
        [SerializeField]
        private Transform[] wayPoints;

        [SerializeField]
        private bool isInAir;
        [SerializeField]
        private float startAltitude;
        [SerializeField]
        private float startSpeed;
        [SerializeField]
        private Runway runway;



        private Formation<AircraftFormationMember> currentFormation;
        private AircraftFacade.Pool aircraftPool;
        private ControllableAircraftManager controllableAircraftManager;
        private FormationCommandSystem formationCommandSystem;
        private CommandManager commandManager;
        private CommandTargetManager commandTargetManager;

        [Inject]
        private void Init(TeamAircraftFactoryProvider factoryProvider, ControllableAircraftManager controllableAircraftManager, FormationCommandSystem formationCommandSystem,
            CommandManager commandManager, CommandTargetManager commandTargetManager)
        {
            this.aircraftPool = factoryProvider.GetPool(team);
            this.controllableAircraftManager = controllableAircraftManager;
            this.formationCommandSystem = formationCommandSystem;
            this.commandManager = commandManager;
            this.commandTargetManager = commandTargetManager;
        }

        private void RegisterAircraftForCommandSystem(AircraftFacade aircraft)
        {
            if (aircraft.Team == Team.Blue)
            {
                commandManager.AddCommandable(aircraft);
            }
            else
            {
                commandTargetManager.AddTarget(aircraft);
            }
        }

        private IEnumerator Start()
        {
            switch (formationType)
            {
                case Formation.Trail:
                    currentFormation = new Trail<AircraftFormationMember>();
                    break;
                case Formation.Echelon:
                    currentFormation = new Echelon<AircraftFormationMember>();
                    break;
                case Formation.ArrowHead:
                    currentFormation = new ArrowHead<AircraftFormationMember>();
                    break;
                case Formation.BattleSpread:
                    currentFormation = new BattleSpread<AircraftFormationMember>();
                    break;
                default:
                    throw new System.ArgumentException("Invalid formation type");
                    break;
            }

            currentFormation.spacing = this.spacing;
            currentFormation.altitudeSpacing = this.altitudeSpacing;
            if (isInAir)
            {
                for (int i = 0; i < count; i++)
                {
                    AircraftFacade newAircraft = aircraftPool.Spawn(position + currentFormation.GetMemberPositionSpaced(i), Quaternion.identity, isInAir, startSpeed);
                    newAircraft.transform.SetParent(transform);
                    yield return null;
                    currentFormation.AddMember(newAircraft.FormationMember.Self);
                    newAircraft.FormationMember.Formation = currentFormation;
                    if(newAircraft.Team == Team.Blue)
                    {
                        controllableAircraftManager.AddControllableAircraft(newAircraft.Aircraft, newAircraft.AIController, null, null, null);
                    }
                    RegisterAircraftForCommandSystem(newAircraft);
                    formationCommandSystem.Register(newAircraft.AIController);
                    newAircraft.AIController.SetWaypoints(GetWaypointPositions());
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    while (runway.IsInUse)
                    {
                        yield return null;
                    }

                    AircraftFacade newAircraft = aircraftPool.Spawn(runway.TouchDownPoint.position, runway.TouchDownPoint.rotation, isInAir, startSpeed);
                    newAircraft.Aircraft.HomeRunway = runway;
                    newAircraft.Aircraft.RunwayInUse = runway;
                    runway.IsInUse = true;
                    newAircraft.transform.SetParent(transform);

                    yield return null;
                    currentFormation.AddMember(newAircraft.FormationMember.Self);
                    newAircraft.FormationMember.Formation = currentFormation;
                    if(newAircraft.Team == Team.Blue)
                    {
                        controllableAircraftManager.AddControllableAircraft(newAircraft.Aircraft, newAircraft.AIController, null, null, null);
                    }
                    RegisterAircraftForCommandSystem(newAircraft);
                    formationCommandSystem.Register(newAircraft.AIController);
                    newAircraft.AIController.SetWaypoints(GetWaypointPositions());
                }
            }

            controllableAircraftManager.AddControllableFormation(currentFormation);
            yield return null;
        }

        private Vector3[] GetWaypointPositions()
        {
            Vector3[] wps = new Vector3[wayPoints.Length];
            for (int i = 0; i < wayPoints.Length; i++)
            {
                wps[i] = wayPoints[i].position;
            }
            return wps;
        }
    }
}
