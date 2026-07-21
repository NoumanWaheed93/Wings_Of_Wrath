using CommandSystem;
using TargetingSystem;

namespace Game.Commands
{
    /// <summary>
    /// Describes one entry in the command palette: its display name, whether picking it
    /// requires a follow-up target selection, and how to build the actual ICommand once
    /// (if required) a target has been chosen.
    /// </summary>
    public interface ICommandDefinition
    {
        public string Name { get; }

        public bool RequiresTarget { get; }

        public ICommand CreateCommand(ITargetable target);
    }

}
