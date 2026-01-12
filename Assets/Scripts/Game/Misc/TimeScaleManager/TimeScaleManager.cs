using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TimeScaleManager : MonoBehaviour
    {
        private const string LOGFORMAT = "<color=#00FF62><b>[TimeScaleManager]</b></color> {0} ";

        private float targetTimeScale = 1f;

        public void SetTimeScale(float targetTimeScale, bool immediate = true)
        {
            if (immediate)
            {
                Time.timeScale = targetTimeScale;
                return;
            }

            this.targetTimeScale = targetTimeScale;
            StopAllCoroutines();
            StartCoroutine(CrtnReachTimeScale());
        }

        private IEnumerator CrtnReachTimeScale()
        {
            Debug.LogFormat(LOGFORMAT, $"CrtnReachTimeScale: Reaching target time scale {targetTimeScale}");
            while (!Mathf.Approximately(Time.timeScale, targetTimeScale))
            {
                Time.timeScale = Mathf.MoveTowards(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime);
                yield return null;
            }
            Time.timeScale = targetTimeScale;
            Debug.LogFormat(LOGFORMAT, $"CrtnReachTimeScale: Reached target time scale {targetTimeScale}");
        }
    }
}
