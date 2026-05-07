using AircraftController;
using Common;
using FormationSystem;
using System;
using UnityEngine;
using Zenject;
using WeaponSystem;
using SelectableEntitySystem;
using Game;

namespace ZenjectInstallers {
    public class TestInstaller : MonoInstaller
    {
        public GameObject AircraftPrefab;

        public GameObject ProjectilePrefab;

        public GameObject GuidedProjectilePrefab;

        public override void InstallBindings()
        {
            InstallTimeProvider();
            InstallFormationManager();
            InstallProjectileFactory();
            InstallSelectableEntityManager();
            InstallPlayerAircraftController();
            InstallControllableAircraftManager();
        }

        private void InstallPlayerAircraftController()
        {
            Container.Bind<IAircraftPlayerInputManager>().To<PlayerKeyboardInputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AircraftPlayerController>().AsSingle();
        }

        private void InstallControllableAircraftManager()
        {
            Container.Bind<ControllableAircraftManager>().AsSingle();
        }

        private void InstallSelectableEntityManager()
        {
            Container.Bind<SelectableEntityManager>().AsSingle();
        }

        private void InstallTimeProvider()
        {
            Container.Bind<ITimeProvider>().To<GameTimeProvider>().AsSingle();
        }

        private void InstallFormationManager()
        {
            Container.Bind<Formation>().To<Trail>().AsSingle();

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

            Container.Bind<IProjectileFactory>().To<Game.ProjectileFactory>().AsSingle();
        }
    }
}
