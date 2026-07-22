using Common;
using Zenject;
using WeaponSystem;
using TargetingSystem;
using UnityEngine;
using AircraftController.AircraftAI;

namespace Game
{
    public class GuidedProjectileLauncherMonobehaviour : WeaponMonoBehaviour, IWeaponController
    {
        private GuidedProjectileLauncher launcher;

        [Inject]
        public void Init(ITimeProvider timeProvider, IProjectileFactory projectileFactory, TargetTracker targetTracker)
        {
            launcher = new GuidedProjectileLauncher(barrelGO.transform, timeProvider, maxAmmo, bulletsPerSecond, projectileFactory);
            weapon = launcher;
            targetTracker.OnSelectTarget += (targetable) => { launcher.Target = targetable.Transform; };
        }

        public void Fire()
        {
            launcher.Fire();
        }

        public bool FireAt(Transform target)
        {
            launcher.Target = target;
            return launcher.Fire();
        }
    }
}
