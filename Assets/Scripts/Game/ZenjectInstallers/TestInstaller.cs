using AircraftController;
using Common;
using FormationSystem;
using System;
using UnityEngine;
using Zenject;
using WeaponSystem;
using SelectableEntitySystem;
using Game;
using UnityEngine.InputSystem;

namespace ZenjectInstallers {
    public class TestInstaller : MonoInstaller
    {
        public GameObject AircraftPrefab;

        public GameObject ProjectilePrefab;

        public GameObject GuidedProjectilePrefab;

        [Space]
        [Header("Player Input Actions")]
        public InputAction SteerAction;
        public InputAction AfterBurnerAction;

        private AircraftPlayerController playerController;

        void Update()
        {
            playerController.Update(Time.deltaTime);    
        }

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
            Container.Bind<IAircraftPlayerInputManager>().To<Game.PlayerInputManager>()
            .AsSingle()
            .WithArguments(SteerAction, AfterBurnerAction);
            Container.Bind<AircraftPlayerController>().AsSingle();
            playerController = Container.Resolve<AircraftPlayerController>();
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
            Container.Bind<Formation>().To<ArrowHead>().AsSingle();

            Container.BindMemoryPool<AircraftFacade, AircraftFacade.Pool>()
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
