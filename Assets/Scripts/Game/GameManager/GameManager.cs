using ScreenInputControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI Observables")]
        [SerializeField]
        private ThumbDriftInput thumbDriftInput;

        [Header("Misc")]
        [SerializeField]
        private TimeScaleManager timeScaleManager;

        [SerializeField]
        private int targetFrameRate = 60;

        [SerializeField]
        private bool isTimeScaleActive = true;

        private void OnEnable()
        {
            Application.targetFrameRate = targetFrameRate;

            if (isTimeScaleActive == false)
            {
                return;
            }
            thumbDriftInput.OnHoldStart += OnThumbDriftHoldStart;
            thumbDriftInput.OnHoldEnd += OnThumbDriftHoldEnd;
        }

        private void OnDisable()
        {
            if (isTimeScaleActive == false)
            {
                return;
            }
            
            thumbDriftInput.OnHoldStart -= OnThumbDriftHoldStart;
            thumbDriftInput.OnHoldEnd -= OnThumbDriftHoldEnd;
        }

        private void OnGUI()
        {
            if (targetFrameRate < 30)
            {
                //write the target frame rate in red
                GUI.color = Color.red;
                GUI.Label(new Rect(10, 10, 200, 20), "Warning Low target frame rate :" + targetFrameRate);
                GUI.color = Color.white;
            }
        }

        private void OnThumbDriftHoldStart()
        {
            timeScaleManager.SetTimeScale(1f, false);
        }

        private void OnThumbDriftHoldEnd()
        {
            timeScaleManager.SetTimeScale(0.2f, false);
        }

    }
}
