using UnityEngine;

namespace WeaponSystem
{
    public interface IProjectileFactory
    {
        public Transform GetProjectile();
        public IGuidedProjectile GetHomingProjectile();
    }
}
