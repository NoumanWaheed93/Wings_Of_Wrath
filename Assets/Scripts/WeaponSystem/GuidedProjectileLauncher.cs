using UnityEngine;
using Common;

namespace WeaponSystem
{
    public class GuidedProjectileLauncher : Weapon
    {

        private IProjectileFactory projectileFactory;

        private Transform target;
        public Transform Target { get => target; set => target = value; }

        public GuidedProjectileLauncher(Transform barrel, ITimeProvider timeProvider, int maximumAmmo, float bulletsPerSecond, IProjectileFactory projectileFactory) : base(barrel, timeProvider, maximumAmmo, bulletsPerSecond)
        {
            this.projectileFactory = projectileFactory;
        }

        public override bool Fire()
        {
            if (base.Fire())
            {
                IHomingProjectile newProjectile = projectileFactory.GetHomingProjectile();
                newProjectile.Transform.position = Barrel.position;
                newProjectile.Transform.rotation = Barrel.rotation;
                newProjectile.Target = target;
                return true;
            }
            return false;
        }
    }
}
