using UnityEngine;
using Common;

namespace AircraftController
{
    public class Runway : MonoBehaviour
    {
        private Team team;
        public Team Team 
        { 
            get => team; 
        }

        private Transform initialApproach;
        public Transform InitialApproach { get => initialApproach; }

        private Transform finalApproach;
        public Transform FinalApproach { get => finalApproach; }

        private Transform touchDownPoint;
        public Transform TouchDownPoint { get => touchDownPoint; }

        //--------------- ChangeableFields------------------------//

        private bool isInUse = false;
        public bool IsInUse
        {
            get
            {
                return isInUse;
            }
            set
            {
                isInUse = value;
            }
        }

        //-------------- Init for testing ----------------------//
        public void Init(Team team, Transform initialApproach, Transform finalApproach, Transform touchDownPoint)
        {
            this.team = team;
            this.initialApproach = initialApproach;
            this.finalApproach = finalApproach;
            this.touchDownPoint = touchDownPoint;
        }

    }

}
