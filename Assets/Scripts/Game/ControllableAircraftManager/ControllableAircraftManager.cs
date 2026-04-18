using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectableEntitySystem;
using AircraftController;
using AircraftController.AircraftAI;

namespace Game
{
    public class ControllableAircraftManager
    {
        private SelectableEntityManager selectableEntityManager;

        private Dictionary<IAircraft, AircraftAIController> dictAircraftAIControllers = new Dictionary<IAircraft, AircraftAIController>();

        private AircraftPlayerController aircraftPlayerController;

        public ControllableAircraftManager(SelectableEntityManager selectableEntityManager)
        {
            this.selectableEntityManager = selectableEntityManager;
        }
    
        public void AddControllableAircraft(IAircraft aircraft)
        {
            AircraftAIController aiController = new AircraftAIController(aircraft, aircraft.Transform);
            dictAircraftAIControllers.Add(aircraft, aiController);
        }

    }

}
