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
            return simpleProjectileFactory.Spawn().Transform;
        }

        public class SimpleProjectileFactory : MonoMemoryPool<ProjectileFacade>
        {
            protected override void OnCreated(ProjectileFacade item)
            {
                base.OnCreated(item);
                item.SetPool(this);
            }

            protected override void OnSpawned(ProjectileFacade item)
            {
                base.OnSpawned(item);
                item.ResetPhysics();
            }
        }

        public class GuidedProjectileFactory : MonoMemoryPool<GuidedProjectileFacade>
        {
            protected override void OnCreated(GuidedProjectileFacade item)
            {
                base.OnCreated(item);
                item.SetPool(this);
            }

            protected override void OnSpawned(GuidedProjectileFacade item)
            {
                base.OnSpawned(item);
                item.ResetPhysics();
            }
        }
    }
}
