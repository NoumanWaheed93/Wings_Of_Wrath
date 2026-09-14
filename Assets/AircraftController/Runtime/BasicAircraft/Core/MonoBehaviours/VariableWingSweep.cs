using UnityEngine;

namespace AircraftController
{
    public class VariableWingSweep : MonoBehaviour
    {
        [SerializeField]
        private Transform leftWing;

        [SerializeField]
        private Transform rightWing;

        [SerializeField]
        private float minSweepAngle = 0f;

        [SerializeField]
        private float maxSweepAngle = 45f;

        private void Start()
        {
            InvokeRepeating(nameof(RandomSweep), 0f, 6f);
        }

        private void RandomSweep()
        {
            float sweep = Random.Range(0f, 1f);
            SetSweep(sweep);
        }

        private void SetSweep(float sweep)
        {
            leftWing.localRotation = Quaternion.Euler(0f, Mathf.Lerp(minSweepAngle, maxSweepAngle, sweep) * -1f, 0f);
            rightWing.localRotation = Quaternion.Euler(0f, Mathf.Lerp(minSweepAngle, maxSweepAngle, sweep), 0f);
        }
    }
}
