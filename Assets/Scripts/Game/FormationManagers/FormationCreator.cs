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

        private Formation currentFormation;
        private AircraftMonoBehaviour.Pool aircraftPool;
        private ControllableAircraftManager controllableAircraftManager;

        [Inject]
        private void Init(AircraftMonoBehaviour.Pool aircraftFactory, Formation formation, ControllableAircraftManager controllableAircraftManager)
        {
            this.aircraftPool = aircraftFactory;
            this.currentFormation = formation;
            this.controllableAircraftManager = controllableAircraftManager;
        }

        private IEnumerator Start()
        {
            currentFormation.spacing = this.spacing;
            currentFormation.altitudeSpacing = this.altitudeSpacing;
            for (int i = 0; i < count; i++)
            {
                AircraftMonoBehaviour newAircraft = aircraftPool.Spawn();
                newAircraft.transform.position = position + currentFormation.GetMemberPositionSpaced(i);
                newAircraft.transform.rotation = Quaternion.identity;
                newAircraft.transform.SetParent(transform);
                yield return null;
                currentFormation.AddMember(newAircraft.FormationMember);
                newAircraft.FormationMember.Formation = currentFormation;
                controllableAircraftManager.AddControllableAircraft(newAircraft.Aircraft, newAircraft.AIController);
                newAircraft.AIController.SetWaypoints(GetWaypointPositions());
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
