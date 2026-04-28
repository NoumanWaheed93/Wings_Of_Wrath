using AircraftController;
using ScreenInputControls;
using UnityEngine;

namespace Game
{
    public class PlayerMobileInputManager : IAircraftPlayerInputManager
    {
        private ThumbDriftInput thumbDriftInput;

        public PlayerMobileInputManager(ThumbDriftInput thumbDriftInput)
        {
            this.thumbDriftInput = thumbDriftInput;
        }

        public float SteerDirection => thumbDriftInput.Direction;

        public bool IsAfterBurnerOn => thumbDriftInput.isHeldDown;

    }

}
