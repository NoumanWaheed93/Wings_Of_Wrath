using Locomotion;
using ScreenInputControls;
using UnityEngine;

namespace Game
{
    public class UIControls : MonoBehaviour
    {
        [Header("Non UI Objects")]
        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private GuidedProjectileLauncherMonobehaviour guidedProjectileLauncher;

        [Space]
        [Header("UI Components")]
        [SerializeField]
        private ThumbDriftInput thumbDriftInput;
        [SerializeField]
        private SpeedView speedView;

        private void Awake()
        {
            thumbDriftInput.Target = playerTransform;
            speedView.SpeedTarget = playerTransform.GetComponent<ISpeedProvider>();
        }

        public void OnClick_FireMissileButton()
        {
            guidedProjectileLauncher.Fire();
        }
    }
}
