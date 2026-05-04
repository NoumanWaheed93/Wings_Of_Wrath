using UnityEngine;

namespace AircraftController
{
    public class InitialApproach : MonoBehaviour
    {
        [SerializeField]
        private Runway runway;

        private void OnTriggerEnter(Collider other)
        {
            AircraftMonoBehaviour landingAircraft = other.GetComponentInParent<AircraftMonoBehaviour>();
            if (landingAircraft == null)
                return;
            if(landingAircraft.Team != runway.Team)
                return;

            //if the aircraft does not have LandingIntent. return
            if (Vector3.Angle(transform.forward, landingAircraft.transform.forward) > GlobalAircraftControllerSettings.maxAngleErrorOnInitialApproach)
                return;

            Debug.Log("Initial Approach done");
            //initial approach done
            landingAircraft.PrepareToLand(runway);
        }

    }
}
