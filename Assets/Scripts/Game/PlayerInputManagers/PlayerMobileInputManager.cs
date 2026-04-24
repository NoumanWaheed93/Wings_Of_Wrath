using AircraftController;
using ScreenInputControls;
using UnityEngine;

namespace Game
{
    public class PlayerMobileInputManager : IAircraftPlayerInputManager
    {
        [SerializeField]
        private ThumbDriftInput thumbDriftInput;

        public float SteerDirection => thumbDriftInput.Direction;

        public bool IsAfterBurnerOn => thumbDriftInput.isHeldDown;

    }

}
