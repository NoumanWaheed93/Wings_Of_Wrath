using AircraftController;
using AircraftController.AircraftAI;
using Common;
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
        private Rigidbody rigidbody;
        [SerializeField]
        private Transform aircraftTransform;
        [SerializeField]
        private bool isAIControlled = true;

        public override void InstallBindings()
        {
            Container.Bind<Team>().FromInstance(Team.Blue).AsSingle();

            InstallHealth();
            InstallRadar();
            InstallAircraft();
        }

        private void InstallAircraft()
        {
            if (isAIControlled)
            {
                Container.BindInterfacesAndSelfTo<Aircraft>().AsSingle()
                    .WithArguments(movementData, aircraftTransform, rigidbody, true, 100.0f, 80.0f);
                Container.BindInterfacesAndSelfTo<AircraftAIController>().AsSingle().WithArguments(aircraftTransform);
            }
            else
            {
                Container.BindInterfacesAndSelfTo<Aircraft>().AsSingle()
                    .WithArguments(movementData, aircraftTransform, rigidbody);
                Container.BindInterfacesAndSelfTo<AircraftPlayerController>().AsSingle();
            }
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
