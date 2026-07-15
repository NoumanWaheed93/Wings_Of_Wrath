using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace TargetingSystem
{
    public class RadarHudView : MonoBehaviour, ITargetTransformReceiver
    {
        [SerializeField]
        private Transform transform_targetsParent;

        [SerializeField]
        private TargetIconView prefab_TargetIcon;

        private RadarMonobehaviour radar;

        private TargetTracker tracker;

        private Dictionary<ITargetable, TargetIconView> targetIcons = new Dictionary<ITargetable, TargetIconView>();

        public Transform Target { set => SetPlayer(value); }

        private void SetPlayer(Transform playerTransform)
        {
            UnsubscribeFromTracker();
            ClearTargetIcons();

            radar = playerTransform.GetComponentInChildren<RadarMonobehaviour>();
            tracker = radar.Tracker;

            tracker.OnAddedTarget += Tracker_OnTargetAdded;
            tracker.OnRemovedTarget += Tracker_OnTargetRemoved;
            tracker.OnSelectTarget += OnSelect_Target;
            tracker.OnDeselectTarget += OnDeselect_Target;
        }

        private void UnsubscribeFromTracker()
        {
            if (tracker == null)
            {
                return;
            }

            tracker.OnAddedTarget -= Tracker_OnTargetAdded;
            tracker.OnRemovedTarget -= Tracker_OnTargetRemoved;
            tracker.OnSelectTarget -= OnSelect_Target;
            tracker.OnDeselectTarget -= OnDeselect_Target;
        }

        private void ClearTargetIcons()
        {
            foreach (TargetIconView targetIcon in targetIcons.Values)
            {
                targetIcon.OnTargetClicked -= OnClick_Target;
                Destroy(targetIcon.gameObject);
            }
            targetIcons.Clear();
        }

        private void Tracker_OnTargetAdded(ITargetable target)
        {
            targetIcons.Add(target, CreateNewTargetIcon(target));
        }

        private void Tracker_OnTargetRemoved(ITargetable target)
        {
            TargetIconView targetIcon = targetIcons[target];
            targetIcon.OnTargetClicked -= OnClick_Target;

            Destroy(targetIcon.gameObject);
            targetIcons.Remove(target);
        }

        private TargetIconView CreateNewTargetIcon(ITargetable target)
        {
            TargetIconView newIcon = Instantiate(prefab_TargetIcon, transform_targetsParent);
            newIcon.Initialize(target, radar.transform);
            newIcon.OnTargetClicked += OnClick_Target;
            return newIcon;
        }

        private void OnClick_Target(ITargetable target)
        {
            radar.OnSelectTarget(target);
        }

        private void OnSelect_Target(ITargetable target)
        {
            targetIcons[target].Highlight();
        }

        private void OnDeselect_Target(ITargetable target)
        {
            targetIcons[target].UnHighlight();
        }
    }
}
