using System;
using System.Collections.Generic;
using TargetingSystem;

namespace Game.Commands
{
    /// <summary>
    /// Roster of entities that can be picked as the target of a command (e.g. an "Attack" order).
    /// Mirrors SelectableEntityManager's add/remove event shape so the UI can list it the same way.
    /// </summary>
    public class CommandTargetManager
    {
        public event Action<ITargetable> OnTargetAdded;
        public event Action<ITargetable> OnTargetRemoved;

        private readonly List<ITargetable> targets = new List<ITargetable>();

        public IReadOnlyList<ITargetable> Targets => targets;

        public void AddTarget(ITargetable target)
        {
            targets.Add(target);
            OnTargetAdded?.Invoke(target);
        }

        public void RemoveTarget(ITargetable target)
        {
            if (targets.Remove(target))
            {
                OnTargetRemoved?.Invoke(target);
            }
        }
    }

}
