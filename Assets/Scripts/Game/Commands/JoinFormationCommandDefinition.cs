using CommandSystem;

namespace Game.Commands
{
    public class JoinFormationCommandDefinition : ICommandDefinition
    {
        public string Name => "Join Formation";

        public bool RequiresTarget => false;

        public ICommand CreateCommand(ICommandTarget target)
        {
            return new JoinFormationCommand();
        }
    }

}
