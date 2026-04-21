using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ScreenInputControls;

namespace AircraftController
{
    public class AircraftPlayerController : IAircraftController, ITickable
    {
        private IAircraft aircraft;
        public IAircraft Aircraft 
        { 
            get 
            { 
                return aircraft; 
            } 
            set 
            { 
                aircraft = value; 
            } 
        }

        private bool isAfterBurnerOn;

        private float turnInput;

        public bool IsAfterBurnerOn { get => isAfterBurnerOn; }
        
        public float AltitudeOffset { get => 0; }

        private ThumbDriftInput inputController;

        public AircraftPlayerController(ThumbDriftInput inputController)
        {
            this.inputController = inputController;
        }

        public float GetDesiredSpeed()
        {
            return 80;
        }

        public float GetTurn()
        {
            return turnInput;
        }

        public void Tick()
        {
            Update(Time.deltaTime);
        }

        public void Update(float simulationDeltaTime)
        {
            if(aircraft == null)
            {
                return;
            }

            turnInput = inputController.Direction;
            isAfterBurnerOn = inputController.isHeldDown;
            aircraft.TurnInput = turnInput;
            aircraft.AfterBurnerInput = isAfterBurnerOn;
            aircraft.DesiredSpeed = 80;
        }
    }
}
