using AircraftController;
using AircraftController.AircraftAI;
using Common;
using Game;
using HealthSystem;
using Locomotion;
using System.Collections;
using System.Collections.Generic;
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

        public override void InstallBindings()
        {
            Container.Bind<Team>().FromInstance(Team.Blue).AsSingle();

            InstallHealth();
            InstallRadar();
            InstallAircraft();
            InstallAircraftAI();
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
    
    }

}
