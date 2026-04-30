using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AircraftController.Debugging
{
    [CustomEditor(typeof(AircraftMonoBehaviour))]
    public class AircraftSpeedDebugger : Editor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        private static void OnSceneGUI(SceneView sceneView)
        {
            // Find all objects with MyMonoBehaviour
            AircraftMonoBehaviour[] allObjects = Object.FindObjectsOfType<AircraftMonoBehaviour>();

            foreach (AircraftMonoBehaviour obj in allObjects)
            {
                // Draw label
                Vector3 labelPos = obj.transform.position + Vector3.up * 0.5f;

                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.magenta },
                    hover = { textColor = Color.red },
                };

                Handles.Label(labelPos, obj.CurrSpeed.ToString(), style);

                // Draw a wire circle
                Handles.color = Color.green;
                Handles.DrawWireDisc(obj.transform.position, Vector3.up, 0.2f);
            }
        }

    }

}
