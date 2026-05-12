using AircraftController;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerControllerZenjectWrapper : ITickable
    {
        AircraftPlayerController playerController;
        public PlayerControllerZenjectWrapper(AircraftPlayerController playerController)
        {
            this.playerController = playerController;
        }

        public void Tick()
        {
            playerController.Update(Time.deltaTime);
        }

    }

}
