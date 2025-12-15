using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    public class RadarHudView : MonoBehaviour
    {
        [SerializeField]
        private Transform transform_targetsParent;

        [SerializeField]
        private TargetIconView prefab_TargetIcon;

        [SerializeField]
        private RadarMonobehaviour radar;

        private TargetTracker tracker;

        private Dictionary<ITargetable, TargetIconView> targetIcons = new Dictionary<ITargetable, TargetIconView>();

        private void OnEnable()
        {
            tracker = radar.Tracker;
            tracker.OnTargetAdded += Tracker_OnTargetAdded;
            tracker.OnTargetRemoved += Tracker_OnTargetRemoved;
        }

        private void Tracker_OnTargetAdded(ITargetable target)
        {
            targetIcons.Add(target, CreateNewTargetIcon(target));
        }

        private void Tracker_OnTargetRemoved(ITargetable target)
        {
            Destroy(targetIcons[target].gameObject);
            targetIcons.Remove(target);
        }

        private TargetIconView CreateNewTargetIcon(ITargetable target)
        {
            TargetIconView newIcon = Instantiate(prefab_TargetIcon, transform_targetsParent);
            newIcon.Initialize(target, radar.transform);
            return newIcon;
        }
    }
}
