using AircraftController;
using Common;
using FormationSystem;
using System;
using UnityEngine;
using Zenject;
using WeaponSystem;

namespace ZenjectInstallers {
    public class TestInstaller : MonoInstaller
    {
        public GameObject AircraftPrefab;

        public GameObject ProjectilePrefab;

        public GameObject GuidedProjectilePrefab;

        public override void InstallBindings()
        {
            //Install aircraft 
            InstallFormationManager();
            InstallProjectileFactory();
        }

        private void InstallFormationManager()
        {
            Container.Bind<Formation>().To<ArrowHead>().AsSingle();

            Container.BindMemoryPool<AircraftMonoBehaviour, AircraftMonoBehaviour.Pool>()
                .FromComponentInNewPrefab(AircraftPrefab)
                .WithGameObjectName("Aircraft")
                .UnderTransformGroup("Aircrafts");
        }

        private void InstallProjectileFactory()
        {
            Container.BindMemoryPool<GuidedProjectile, Game.ProjectileFactory.GuidedProjectileFactory>()
                .FromComponentInNewPrefab(GuidedProjectilePrefab)
                .WithGameObjectName("GuidedProjectile")
                .UnderTransformGroup("GuidedProjectiles");

            Container.BindMemoryPool<Projectile, Game.ProjectileFactory.SimpleProjectileFactory>()
               .FromComponentInNewPrefab(ProjectilePrefab)
               .WithGameObjectName("Projectile")
               .UnderTransformGroup("Projectiles");
        }
    }
}
