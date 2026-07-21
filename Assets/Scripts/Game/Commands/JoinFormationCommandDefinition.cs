using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    public class JoinFormationCommandDefinition : ICommandDefinition
    {
        public string Name => "Join Formation";

        public bool RequiresTarget => false;

        public ICommand CreateCommand(ITargetable target)
        {
            return new JoinFormationCommand();
        }
    }

}
