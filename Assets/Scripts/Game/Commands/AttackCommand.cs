using CommandSystem;
using TargetingSystem;
using UnityEngine;

namespace Game.Commands
{
    /// <summary>
    /// Demo-level "engage" order: sends the commandable to intercept the target's current
    /// position by reusing the existing waypoint-following AI state. Not a combat system.
    /// </summary>
    public class AttackCommand : ICommand
    {
        private readonly ITargetable target;

        public AttackCommand(ITargetable target)
        {
            this.target = target;
        }

        public void Execute(ICommandable commandable)
        {
            if (commandable is AircraftFacade aircraftFacade)
            {
                aircraftFacade.AIController.SetWaypoints(new Vector3[] { target.Transform.position });
            }
        }
        
    }

}
