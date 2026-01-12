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


        private void OnEnable()
        {
            thumbDriftInput.OnHoldStart += OnThumbDriftHoldStart;
            thumbDriftInput.OnHoldEnd += OnThumbDriftHoldEnd;
        }

        private void OnDisable()
        {
            thumbDriftInput.OnHoldStart -= OnThumbDriftHoldStart;
            thumbDriftInput.OnHoldEnd -= OnThumbDriftHoldEnd;
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
