using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    public class TargetIconView : MonoBehaviour
    {
        private ITargetable target;

        public void Initialize(ITargetable target)
        {
            this.target = target;
        }

        private void Update()
        {
            transform.position = target.Transform.position;
            transform.rotation = target.Transform.rotation;
        }
    }
}
