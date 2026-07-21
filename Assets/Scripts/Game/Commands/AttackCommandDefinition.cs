using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    public class AttackCommandDefinition : ICommandDefinition
    {
        public string Name => "Attack";

        public bool RequiresTarget => true;

        public ICommand CreateCommand(ICommandTarget target)
        {
            return new AttackCommand((ITargetable)target);
        }
    }

}
