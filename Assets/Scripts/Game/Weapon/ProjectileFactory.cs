using UnityEngine;
using WeaponSystem;
using Zenject;

namespace Game
{
    public class ProjectileFactory : IProjectileFactory
    {
        private SimpleProjectileFactory simpleProjectileFactory;
        private GuidedProjectileFactory guidedProjectileFactory;

        public ProjectileFactory(SimpleProjectileFactory simpleProjectileFactory, GuidedProjectileFactory guidedProjectileFactory)
        {
            this.simpleProjectileFactory = simpleProjectileFactory;
            this.guidedProjectileFactory = guidedProjectileFactory;
        }

        public IGuidedProjectile GetHomingProjectile()
        {
            return guidedProjectileFactory.Spawn();
        }

        public Transform GetProjectile()
        {
            return simpleProjectileFactory.Spawn().transform;
        }

        public class SimpleProjectileFactory : MonoMemoryPool<Projectile>
        {

        }

        public class GuidedProjectileFactory : MonoMemoryPool<GuidedProjectile> 
        {
            
        }
    }
}
