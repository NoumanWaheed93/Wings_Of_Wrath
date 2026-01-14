using Locomotion;
using NomiUIExtensions;
using ScreenInputControls;
using UnityEngine;
using WeaponSystem;

namespace Game
{
    public class UIControls : MonoBehaviour
    {
        [Header("Non UI Objects")]
        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private GuidedProjectileLauncherMonobehaviour guidedProjectileLauncher;

        [SerializeField]
        private RaycastGunMonobehaviourDemo gun;

        [Space]
        [Header("UI Components")]
        [SerializeField]
        private ThumbDriftInput thumbDriftInput;
        [SerializeField]
        private SpeedView speedView;
        [SerializeField]
        private SurroundingContextMenu aircraftControlsMenu;

        private void Awake()
        {
            thumbDriftInput.Target = playerTransform;
            speedView.SpeedTarget = playerTransform.GetComponent<ISpeedProvider>();
        }

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
            aircraftControlsMenu.Hide();
        }

        private void OnThumbDriftHoldEnd()
        {
            aircraftControlsMenu.ShowUp();
        }

        public void OnClick_FireMissileButton()
        {
            guidedProjectileLauncher.Fire();
        }

        public void OnHold_FireGunButton()
        {
            gun.Fire();
        }
    }
}
