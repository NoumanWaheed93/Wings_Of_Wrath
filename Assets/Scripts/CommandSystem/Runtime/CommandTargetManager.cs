using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem
{
    public class CommandTargetManager
    {
    
        public event Action<ICommandTarget> OnTargetAdded;
        public event Action<ICommandTarget> OnTargetRemoved;

        private readonly List<ICommandTarget> targets = new List<ICommandTarget>();

        public IReadOnlyList<ICommandTarget> Targets => targets;

        public void AddTarget(ICommandTarget target)
        {
            targets.Add(target);
            OnTargetAdded?.Invoke(target);
        }

        public void RemoveTarget(ICommandTarget target)
        {
            if (targets.Remove(target))
            {
                OnTargetRemoved?.Invoke(target);
            }
        }
        
    }
}
