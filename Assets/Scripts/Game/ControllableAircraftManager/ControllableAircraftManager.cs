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

        public ControllableAircraftManager(SelectableEntityManager selectableEntityManager, ThumbDriftInput thumbDrift, UIControls uiControls, TopDownCamera cameraController)
        {
            this.selectableEntityManager = selectableEntityManager;
            this.aircraftPlayerController = new AircraftPlayerController(thumbDrift);

            this.uiControls = uiControls;
            this.cameraController = cameraController;
            this.selectableEntityManager.OnEntitySelected += SelectableEntityManager_OnEntitySelected;
        }

        public void AddControllableAircraft(IAircraft aircraft)
        {
            AircraftAIController aiController = new AircraftAIController(aircraft, aircraft.Transform);
            SelectableEntity selectableEntity = new SelectableEntity("aircraft");
            selectableEntityManager.AddSelectableEntity(selectableEntity);
            ControllableAircraft controllableAircraft = new ControllableAircraft
            {
                aircraft = aircraft,
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

            bool isControlGivenToThePlayer = false;
            bool isControlGivenToTheAI = false;
            for (int i = 0; i < controllableAircrafts.Count; i++)
            {
                if (controllableAircrafts[i].aircraft == aircraftPlayerController.Aircraft)
                {
                    aircraftPlayerController.Aircraft = null;
                    controllableAircrafts[i].controller.IsActive = true;
                    isControlGivenToTheAI = true;
                }
                else if (controllableAircrafts[i].selectableEntity == aircraftEntity)
                {
                    controllableAircrafts[i].controller.IsActive = false;
                    aircraftPlayerController.Aircraft = controllableAircrafts[i].aircraft;
                    cameraController.Target = controllableAircrafts[i].aircraft.Transform;
                    uiControls.SetPlayer(cameraController.Target, cameraController.GetComponentInChildren<MeshRenderer>());
                    isControlGivenToThePlayer = true;
                }

                if(isControlGivenToTheAI && isControlGivenToThePlayer)
                {
                    return;
                }
            }
        }

    }

}
