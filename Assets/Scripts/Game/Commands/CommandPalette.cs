using System.Collections.Generic;

namespace Game.Commands
{
    /// <summary>
    /// The fixed set of commands offered by the demo's command palette UI.
    /// </summary>
    public class CommandPalette
    {
        private readonly List<ICommandDefinition> definitions = new List<ICommandDefinition>
        {
            new JoinFormationCommandDefinition(),
            new BreakFormationCommandDefinition(),
            new AttackCommandDefinition(),
        };

        public IReadOnlyList<ICommandDefinition> Definitions => definitions;
    }

}
