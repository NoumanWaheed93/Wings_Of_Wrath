using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class ArcLineRenderer : MonoBehaviour
    {
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.useWorldSpace = true;
                DrawPredictedPath();
            }
        }

        [Header("Motion Inputs")]
        public float linearVelocity = 2f;            // units per second
        public float angularVelocity = 0.5f;          // radians per second (Y-axis)

        [Header("Prediction Settings")]
        public float predictionTime = 3f;             // seconds
        public float timeStep = 0.05f;                 // seconds

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
        }

        private void Update()
        {
            DrawPredictedPath();
        }

        private void DrawPredictedPath()
        {
            int steps = Mathf.CeilToInt(predictionTime / timeStep);
            lineRenderer.positionCount = steps + 1;

            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            lineRenderer.SetPosition(0, position);

            for (int i = 1; i <= steps; i++)
            {
                // Integrate rotation
                float deltaAngle = angularVelocity * timeStep;
                rotation *= Quaternion.Euler(0f, deltaAngle * Mathf.Rad2Deg, 0f);

                // Integrate position
                Vector3 forward = rotation * Vector3.forward;
                position += forward * linearVelocity * timeStep;

                lineRenderer.SetPosition(i, position);
            }
        }
    }
}
