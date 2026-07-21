using CommandSystem;

namespace Game.Commands
{
    /// <summary>
    /// Same effect as FormationCommandSystem.CommandBreakFormation, but issued per-selection
    /// through the generic CommandSystem instead of broadcast to every registered aircraft.
    /// </summary>
    public class BreakFormationCommand : ICommand
    {
        public void Execute(ICommandable commandable)
        {
            if (commandable is AircraftFacade aircraftFacade)
            {
                aircraftFacade.AIController.IsFormationBreaking = true;
            }
        }
    }

}
