using System.Collections.Generic;

namespace CommandSystem
{
    public interface ICommandPallette
    {
        public IReadOnlyList<ICommandDefinition> Definitions { get; }
    }

}
