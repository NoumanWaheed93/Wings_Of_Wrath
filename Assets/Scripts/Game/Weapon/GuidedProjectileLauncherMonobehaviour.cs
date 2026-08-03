using Zenject;
using WeaponSystem;
using TargetingSystem;

namespace Game
{
    public class GuidedProjectileLauncherMonobehaviour : WeaponMonoBehaviour
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
    }
}
