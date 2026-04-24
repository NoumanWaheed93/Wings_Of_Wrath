using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectableEntitySystem;
using AircraftController;
using AircraftController.AircraftAI;
using ScreenInputControls;
using CameraController;

namespace Game
{
    public class ControllableAircraftManager
    {
        private struct ControllableAircraft
        {
            public IAircraft aircraft;
            public AircraftAIController controller;
            public SelectableEntity selectableEntity;
        }

        private List<ControllableAircraft> controllableAircrafts = new List<ControllableAircraft>();

        private SelectableEntityManager selectableEntityManager;

        private AircraftPlayerController aircraftPlayerController;

        private TopDownCamera cameraController;
        private UIControls uiControls;

        public ControllableAircraftManager(SelectableEntityManager selectableEntityManager, AircraftPlayerController playerController, UIControls uiControls, TopDownCamera cameraController)
        {
            this.selectableEntityManager = selectableEntityManager;
            this.uiControls = uiControls;
            this.aircraftPlayerController = playerController;
            this.cameraController = cameraController;
            this.selectableEntityManager.OnEntitySelected += SelectableEntityManager_OnEntitySelected;
        }

        public void AddControllableAircraft(AircraftAIController aiController)
        {
            SelectableEntity selectableEntity = new SelectableEntity("aircraft");
            selectableEntityManager.AddSelectableEntity(selectableEntity);
            ControllableAircraft controllableAircraft = new ControllableAircraft
            {
                aircraft = aiController.aircraft,
                controller = aiController,
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
                    cameraController.Target = controllableAircrafts[i].aircraft.Transform;
                    uiControls.SetPlayer(cameraController.Target);
                    break;
                }
            }
        }

    }

}
