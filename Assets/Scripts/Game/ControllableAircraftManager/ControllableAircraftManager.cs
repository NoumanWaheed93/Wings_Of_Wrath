using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectableEntitySystem;
using AircraftController;
using AircraftController.AircraftAI;
using Common;
using WeaponSystem;
using FormationSystem;

namespace Game
{
    public class ControllableAircraftManager
    {
        private struct ControllableAircraft
        {
            public IAircraft aircraft;
            public IAircraftController controller;
            public Weapon cannon;
            public Weapon missileLauncher;
            public Weapon bombDropper;
        }

        private List<ControllableAircraft> controllableAircrafts = new List<ControllableAircraft>();

        private Dictionary<SelectableEntity, Formation<AircraftFormationMember>> formations = new Dictionary<SelectableEntity, Formation<AircraftFormationMember>>();
        
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

        public void AddControllableFormation(Formation<AircraftFormationMember> formation)
        {
            SelectableEntity selectableEntity = new SelectableEntity("formation");
            selectableEntityManager.AddSelectableEntity(selectableEntity);
            formations[selectableEntity] = formation;
        }

        public void AddControllableAircraft(IAircraft aircraft, IAircraftController aiController, 
        Weapon cannon, Weapon missileLauncher, Weapon bombDropper)
        {
            
            ControllableAircraft controllableAircraft = new ControllableAircraft
            {
                aircraft = aircraft,
                controller = aiController,
                cannon = cannon,
                missileLauncher = missileLauncher,
                bombDropper = bombDropper
            };
            controllableAircrafts.Add(controllableAircraft);
        }

        private void SelectableEntityManager_OnEntitySelected(SelectableEntity entity) 
        {
            GiveAircraftControlToPlayer(entity);
        }

        private void GiveAircraftControlToPlayer(SelectableEntity formationEntity)
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

            IAircraft formationLeader = formations[formationEntity].leader.aircraft;

            for (int i = 0; i < controllableAircrafts.Count; i++)
            {
                if (controllableAircrafts[i].aircraft == formationLeader)
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
