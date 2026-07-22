using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    /// <summary>
    /// Orders the commandable to engage the target: it enters the long range engagement AI
    /// state, fires a guided missile at the target, then rejoins its formation.
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
                aircraftFacade.AIController.EngageTarget(target.Transform);
            }
        }
    }

}
