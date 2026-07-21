using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    public class AttackCommandDefinition : ICommandDefinition
    {
        public string Name => "Attack";

        public bool RequiresTarget => true;

        public ICommand CreateCommand(ITargetable target)
        {
            return new AttackCommand(target);
        }
    }

}
