using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AircraftController.AircraftAI;

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

        private static System.Type FindType(string typeName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            // Find all objects with MyMonoBehaviour
            AircraftMonoBehaviour[] allObjects = Object.FindObjectsByType<AircraftMonoBehaviour>();

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

                string speedText = $"Speed: {obj.CurrSpeed:F1}";
                
                string physStateText = "Phys State: None";
                if (obj.Aircraft?.StateMachine?.currentState != null)
                {
                    physStateText = $"Phys State: {obj.Aircraft.StateMachine.currentState.GetType().Name}";
                }

                string aiStateText = "AI State: None";
                System.Type facadeType = FindType("Game.AircraftFacade");
                Component facade = null;
                if (facadeType != null)
                {
                    facade = obj.GetComponent(facadeType) ?? obj.GetComponentInParent(facadeType) ?? obj.GetComponentInChildren(facadeType);
                }

                if (facade != null)
                {
                    object aiControllerObj = facade.GetType()
                        .GetProperty("AIController", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(facade);

                    if (aiControllerObj == null)
                    {
                        aiControllerObj = facade.GetType()
                            .GetField("aiController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            ?.GetValue(facade);
                    }

                    if (aiControllerObj is AircraftAIController aiController)
                    {
                        if (aiController.StateMachine?.currentState != null)
                        {
                            var currentState = aiController.StateMachine.currentState;
                            aiStateText = $"AI State: {currentState.GetType().Name}";
                            
                            if (currentState is StateLanding landingState)
                            {
                                var phaseField = typeof(StateLanding).GetField("phase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                if (phaseField != null)
                                {
                                    var phaseVal = phaseField.GetValue(landingState);
                                    if (phaseVal != null)
                                    {
                                        aiStateText += $" ({phaseVal})";
                                    }
                                }
                            }
                        }
                    }
                }

                string text = $"{speedText}\n{physStateText}\n{aiStateText}";
                Handles.Label(labelPos, text, style);

                // Draw a wire circle
                Handles.color = Color.green;
                Handles.DrawWireDisc(obj.transform.position, Vector3.up, 0.2f);
            }
        }

    }

}
