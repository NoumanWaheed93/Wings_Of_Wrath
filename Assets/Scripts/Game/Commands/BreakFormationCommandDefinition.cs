using CommandSystem;

namespace Game.Commands
{
    public class BreakFormationCommandDefinition : ICommandDefinition
    {
        public string Name => "Break Formation";

        public bool RequiresTarget => false;

        public ICommand CreateCommand(ICommandTarget target)
        {
            return new BreakFormationCommand();
        }
    }

}
