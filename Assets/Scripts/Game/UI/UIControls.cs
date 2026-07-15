using Common;
using Locomotion;
using NomiUIExtensions;
using ScreenInputControls;
using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;
using Zenject;

namespace Game
{
    public class UIControls : MonoBehaviour, ITargetTransformReceiver
    {
        [Space]
        [Header("UI Components")]
        [Inject]
        private ThumbDriftInput thumbDriftInput;
        [SerializeField]
        private SpeedView speedView;
        [SerializeField]
        private SurroundingContextMenu aircraftControlsMenu;
        [SerializeField]
        private FireHoldableButton fireHoldableButton;
        [SerializeField]
        private Button btnMissile;


        private GuidedProjectileLauncherMonobehaviour guidedProjectileLauncher;

        private RaycastGunMonobehaviourDemo gun;

        public Transform Target { set => SetPlayer(value); }

        private void OnEnable()
        {
            thumbDriftInput.OnHoldStart += OnThumbDriftHoldStart;
            fireHoldableButton.onHold.AddListener(OnHold_FireGunButton);
            thumbDriftInput.OnHoldEnd += OnThumbDriftHoldEnd;
            btnMissile.onClick.AddListener(OnClick_FireMissileButton);
        }

        private void OnDisable()
        {
            thumbDriftInput.OnHoldStart -= OnThumbDriftHoldStart;
            fireHoldableButton.onHold.RemoveListener(OnHold_FireGunButton);
            thumbDriftInput.OnHoldEnd -= OnThumbDriftHoldEnd;
            btnMissile.onClick.RemoveListener(OnClick_FireMissileButton);
        }

        private void SetPlayer(Transform playerTransform)
        {
            thumbDriftInput.Target = playerTransform;
            speedView.SpeedTarget = playerTransform.GetComponent<ISpeedProvider>();

            MeshRenderer playerMeshRenderer = playerTransform.GetComponentInChildren<MeshRenderer>();
            aircraftControlsMenu.SetMeshRenderer(playerMeshRenderer);

            this.guidedProjectileLauncher = playerTransform.GetComponentInChildren<GuidedProjectileLauncherMonobehaviour>();
            this.gun = playerTransform.GetComponentInChildren<RaycastGunMonobehaviourDemo>();
        }

        private void OnThumbDriftHoldStart()
        {
            aircraftControlsMenu.Hide();
        }

        private void OnThumbDriftHoldEnd()
        {
            aircraftControlsMenu.ShowUp();
        }

        private void OnClick_FireMissileButton()
        {
            guidedProjectileLauncher.Fire();
        }

        private void OnHold_FireGunButton()
        {
            gun.Fire();
        }
    
    }

}
