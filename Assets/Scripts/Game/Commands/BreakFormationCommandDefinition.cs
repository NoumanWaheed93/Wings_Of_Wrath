using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    public class BreakFormationCommandDefinition : ICommandDefinition
    {
        public string Name => "Break Formation";

        public bool RequiresTarget => false;

        public ICommand CreateCommand(ITargetable target)
        {
            return new BreakFormationCommand();
        }
    }

}
