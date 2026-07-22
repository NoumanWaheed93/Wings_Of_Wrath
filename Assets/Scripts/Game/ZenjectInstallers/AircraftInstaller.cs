using AircraftController;
using AircraftController.AircraftAI;
using Common;
using HealthSystem;
using Locomotion;
using TargetingSystem;
using UnityEngine;
using Zenject;

namespace ZenjectInstallers
{
    public class AircraftInstaller : MonoInstaller
    {
        [SerializeField]
        private AircraftMovementData movementData;
        [SerializeField]
        private new Rigidbody rigidbody;
        [SerializeField]
        private Transform aircraftTransform;
        [SerializeField]
        private Team team;


        public override void InstallBindings()
        {
            Container.Bind<Team>().FromInstance(team).AsSingle();

            InstallHealth();
            InstallRadar();
            InstallFormationMember();
            InstallWeapons();
            InstallAircraft();
            InstallAircraftAI();
        }

        private void InstallWeapons()
        {
            // Optional: aircraft prefabs without a weapon simply have no IWeaponController bound,
            // and the AI controller treats firing as a no-op (see AircraftAIController.FireAt).
            Container.Bind<IWeaponController>().FromComponentInHierarchy().AsSingle();
        }

        private void InstallAircraftAI()
        {
            Container.BindInterfacesAndSelfTo<AircraftAIController>().AsSingle().WithArguments(aircraftTransform).NonLazy();
        }

        private void InstallAircraft()
        {
            Container.BindInterfacesAndSelfTo<Aircraft>().AsSingle()
                .WithArguments(movementData, aircraftTransform, rigidbody);
        }

        private void InstallHealth()
        {
            Health health = new Health(100, 100);
            Container.Bind<Health>().FromInstance(health).AsSingle();
        }

        private void InstallRadar()
        {
            Container.Bind<TargetTracker>().AsSingle();
        }

        private void InstallFormationMember()
        {
            Container.BindInterfacesAndSelfTo<AircraftFormationMember>().AsSingle();
        }

    }

}
