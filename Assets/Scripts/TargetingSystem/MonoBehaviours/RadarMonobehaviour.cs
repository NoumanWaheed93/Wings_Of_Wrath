using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    [RequireComponent(typeof(Collider))]
    public class RadarMonobehaviour : MonoBehaviour
    {
        private TargetTracker tracker = new TargetTracker();

        public TargetTracker Tracker => tracker;

        private List<Rigidbody> colliders = new List<Rigidbody>();

        private void OnTriggerEnter(Collider other)
        {
            if(other.attachedRigidbody != null && colliders.Contains(other.attachedRigidbody))
            {
                return;
            }

            colliders.Add(other.attachedRigidbody);

            ITargetable target = other.GetComponentInParent<ITargetable>();
            if (target != null)
            {
                tracker.AddTarget(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody != null && colliders.Contains(other.attachedRigidbody))
            {
                colliders.Remove(other.attachedRigidbody);

                ITargetable target = other.GetComponentInParent<ITargetable>();
                if (target != null)
                {
                    tracker.RemoveTarget(target);
                }
            }
        }
    
        public void OnSelectTarget(ITargetable target)
        {
            Tracker.SelectedTarget = target;
        }
    }
}
