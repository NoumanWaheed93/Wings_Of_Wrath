using Common;
using UnityEngine;

namespace WeaponSystem
{
    public class ProjectileLauncher : Weapon
    {
        private IProjectileFactory projectileFactory;
        
        public ProjectileLauncher(Transform barrel, ITimeProvider timeProvider, int maximumAmmo, float bulletsPerSecond, IProjectileFactory projectileFactory):base(barrel, timeProvider, maximumAmmo, bulletsPerSecond)
        {
            this.projectileFactory = projectileFactory;
        }

        public override bool Fire()
        {
            if (base.Fire())
            {
                Transform newProjectile = projectileFactory.GetProjectile(); // GameObject.Instantiate<Projectile>(projectile);
                newProjectile.position = Barrel.position; // newProjectile.transform.position = Barrel.position;
                newProjectile.rotation = Barrel.rotation; // newProjectile.transform.rotation = Barrel.rotation;
                return true;
            }
            return false;
        }
    }
}
