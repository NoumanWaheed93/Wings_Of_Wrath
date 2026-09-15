using UnityEngine;

namespace AircraftController
{
    public class VariableWingSweep : MonoBehaviour
    {
        [SerializeField]
        private AircraftMonoBehaviour controller;

        [SerializeField]
        private Transform leftWing;

        [SerializeField]
        private Transform rightWing;

        [SerializeField]
        private float minSweepAngle = 0f;

        [SerializeField]
        private float maxSweepAngle = 45f;

        [SerializeField]
        private float minSweepSpeed = 30f;

        [SerializeField]
        private float maxSweepSpeed = 70f;

        private void Update()
        {
            SetSweepAccordingToSpeed(controller.CurrSpeed);
        }

        private void SetSweepAccordingToSpeed(float speed)
        {
            float sweep = Mathf.InverseLerp(minSweepSpeed, maxSweepSpeed, speed);
            SetSweep(sweep);
        }

        private void SetSweep(float sweep)
        {
            leftWing.localRotation = Quaternion.Euler(0f, Mathf.Lerp(minSweepAngle, maxSweepAngle, sweep) * -1f, 0f);
            rightWing.localRotation = Quaternion.Euler(0f, Mathf.Lerp(minSweepAngle, maxSweepAngle, sweep), 0f);
        }
    }
}
