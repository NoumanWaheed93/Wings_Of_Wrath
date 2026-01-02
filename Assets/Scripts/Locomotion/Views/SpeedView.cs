using UnityEngine;

namespace Locomotion
{
    public class SpeedView : MonoBehaviour
    {
        [SerializeField]
        private float maxSpeed;

        [SerializeField]
        private Transform needle;

        private ISpeedProvider speedTarget;
        public ISpeedProvider SpeedTarget
        {
            get { return speedTarget; }
            set { speedTarget = value; }
        }

        private float currSpeed;

        private void Update()
        {
            currSpeed = speedTarget.CurrSpeed;
            needle.transform.localRotation = Quaternion.Euler(0, 0, -180 * (currSpeed / maxSpeed));
        }
    }
}
