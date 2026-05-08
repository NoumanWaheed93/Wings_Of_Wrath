using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectableEntitySystem;
using AircraftController;
using AircraftController.AircraftAI;
using Common;
using WeaponSystem;

namespace Game
{
    public class ControllableAircraftManager
    {
        private struct ControllableAircraft
        {
            public IAircraft aircraft;
            public IAircraftController controller;
            public SelectableEntity selectableEntity;
            public Weapon cannon;
            public Weapon missileLauncher;
            public Weapon bombDropper;
        }

        private List<ControllableAircraft> controllableAircrafts = new List<ControllableAircraft>();

        private SelectableEntityManager selectableEntityManager;

        private AircraftPlayerController aircraftPlayerController;

        private ITargetTransformReceiver[] targetReceivers;

        public ControllableAircraftManager(SelectableEntityManager selectableEntityManager, AircraftPlayerController playerController, ITargetTransformReceiver[] targetReceivers)
        {
            this.selectableEntityManager = selectableEntityManager;
            this.targetReceivers = targetReceivers;
            this.aircraftPlayerController = playerController;
            this.selectableEntityManager.OnEntitySelected += SelectableEntityManager_OnEntitySelected;
        }

        public void AddControllableAircraft(IAircraft aircraft, IAircraftController aiController, 
        Weapon cannon, Weapon missileLauncher, Weapon bombDropper)
        {
            SelectableEntity selectableEntity = new SelectableEntity("aircraft");
            selectableEntityManager.AddSelectableEntity(selectableEntity);
            ControllableAircraft controllableAircraft = new ControllableAircraft
            {
                aircraft = aircraft,
                controller = aiController,
                cannon = cannon,
                missileLauncher = missileLauncher,
                bombDropper = bombDropper,
                selectableEntity = selectableEntity
            };
            controllableAircrafts.Add(controllableAircraft);
        }

        private void SelectableEntityManager_OnEntitySelected(SelectableEntity entity) 
        {
            GiveAircraftControlToPlayer(entity);
        }

        private void GiveAircraftControlToPlayer(SelectableEntity aircraftEntity)
        {
            Debug.Log("GiveAircraftControlToPlayer(SelectableEntity)");

            for (int i = 0; i < controllableAircrafts.Count; i++)
            {
                if (controllableAircrafts[i].aircraft == aircraftPlayerController.Aircraft)
                {
                    aircraftPlayerController.Aircraft = null;
                    controllableAircrafts[i].controller.IsActive = true;
                    break;
                }
            }

            for (int i = 0; i < controllableAircrafts.Count; i++)
            {
                if (controllableAircrafts[i].selectableEntity == aircraftEntity)
                {
                    controllableAircrafts[i].controller.IsActive = false;
                    aircraftPlayerController.Aircraft = controllableAircrafts[i].aircraft;
                    foreach(ITargetTransformReceiver targetReceiver in this.targetReceivers)
                    {
                        targetReceiver.Target = controllableAircrafts[i].aircraft.Transform;
                    }
                    break;
                }
            }
        }

    }

}
