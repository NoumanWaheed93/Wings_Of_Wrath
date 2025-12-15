using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    public class TargetIconView : MonoBehaviour
    {
        private ITargetable target;
        private Transform radarTransform;

        public void Initialize(ITargetable target, Transform radarTransform)
        {
            this.target = target;
            this.radarTransform = radarTransform;
        }

        private void LateUpdate()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, target.Transform.position);
        }

        private void UpdateRotation()
        {
            Vector3 relativeDirection = radarTransform.InverseTransformDirection(target.Transform.forward);
            float angle = Mathf.Atan2(relativeDirection.x, relativeDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}
