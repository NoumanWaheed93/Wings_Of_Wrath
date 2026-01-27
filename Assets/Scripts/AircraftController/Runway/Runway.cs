using UnityEngine;
using Common;

namespace AircraftController
{
    public class Runway : MonoBehaviour
    {
        [SerializeField]
        private Team team;
        public Team Team 
        { 
            get => team; 
        }

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

        [SerializeField]
        private Transform initialApproach;
        public Transform InitialApproach { get => initialApproach; }

        [SerializeField]
        private Transform finalApproach;
        public Transform FinalApproach { get => finalApproach; }

        [SerializeField]
        private Transform touchDownPoint;
        public Transform TouchDownPoint { get => touchDownPoint; }

    }
}
