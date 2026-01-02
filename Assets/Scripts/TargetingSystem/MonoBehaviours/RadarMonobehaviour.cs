using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    [RequireComponent(typeof(Collider))]
    public class RadarMonobehaviour : MonoBehaviour
    {
        private TargetTracker tracker;

        public TargetTracker Tracker => tracker;

        private List<Rigidbody> trackedRigidbodies = new List<Rigidbody>();

        public void Init(TargetTracker tracker)
        {
            this.tracker = tracker;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.attachedRigidbody != null && trackedRigidbodies.Contains(other.attachedRigidbody))
            {
                return;
            }

            trackedRigidbodies.Add(other.attachedRigidbody);

            ITargetable target = other.GetComponentInParent<ITargetable>();
            if (target != null)
            {
                tracker.AddTarget(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody != null && trackedRigidbodies.Contains(other.attachedRigidbody))
            {
                trackedRigidbodies.Remove(other.attachedRigidbody);

                ITargetable target = other.GetComponentInParent<ITargetable>();
                if (target != null)
                {
                    tracker.RemoveTarget(target);
                }
            }
        }
    
        public void OnSelectTarget(ITargetable target)
        {
            tracker.SelectedTarget = target;
        }
    }
}
