using System.Collections;
using System.Collections.Generic;
using ScreenInputControls;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Game
{
    public class ThumbDrift_Action : OnScreenControl
    {
        [SerializeField]
        private ThumbDriftInput thumbDriftInput;

        void Update()
        {
          SendValueToControl(thumbDriftInput.Direction);  
        }

        [InputControl(layout = "Axis")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }

}
