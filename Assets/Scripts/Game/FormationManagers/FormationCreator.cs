using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormationSystem;
using Zenject;
using AircraftController.AircraftAI;
using AircraftController;

namespace Game
{
    public class FormationCreator : MonoBehaviour
    {
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

        [Inject]
        private void Init(AircraftFacade.Pool aircraftFactory, Formation<AircraftFormationMember> formation, ControllableAircraftManager controllableAircraftManager)
        {
            this.aircraftPool = aircraftFactory;
            this.currentFormation = formation;
            this.controllableAircraftManager = controllableAircraftManager;
        }

        private IEnumerator Start()
        {
            currentFormation.spacing = this.spacing;
            currentFormation.altitudeSpacing = this.altitudeSpacing;
            if (isInAir)
            {
                for (int i = 0; i < count; i++)
                {
                    AircraftFacade newAircraft = aircraftPool.Spawn(isInAir, startAltitude, startSpeed);
                    newAircraft.transform.position = position + currentFormation.GetMemberPositionSpaced(i);
                    newAircraft.transform.rotation = Quaternion.identity;
                    newAircraft.transform.SetParent(transform);
                    yield return null;
                    currentFormation.AddMember(newAircraft.FormationMember.Self);
                    newAircraft.FormationMember.Formation = currentFormation;
                    controllableAircraftManager.AddControllableAircraft(newAircraft.Aircraft, newAircraft.AIController, null, null, null);
                    newAircraft.AIController.SetWaypoints(GetWaypointPositions());
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    while (runway.IsInUse)
                    {
                        Debug.Log("Waiting for the runway to be free.");
                        yield return null;
                    }

                    AircraftFacade newAircraft = aircraftPool.Spawn(isInAir, startAltitude, startSpeed);
                    newAircraft.Aircraft.HomeRunway = runway;
                    newAircraft.Aircraft.RunwayInUse = runway;
                    runway.IsInUse = true;
                    newAircraft.transform.position = runway.TouchDownPoint.position;
                    newAircraft.transform.rotation = runway.TouchDownPoint.rotation;
                    newAircraft.transform.SetParent(transform);
                    
                    yield return null;
                    currentFormation.AddMember(newAircraft.FormationMember.Self);
                    newAircraft.FormationMember.Formation = currentFormation;
                    controllableAircraftManager.AddControllableAircraft(newAircraft.Aircraft, newAircraft.AIController, null, null, null);
                    newAircraft.AIController.SetWaypoints(GetWaypointPositions());
                }
            }
            
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
