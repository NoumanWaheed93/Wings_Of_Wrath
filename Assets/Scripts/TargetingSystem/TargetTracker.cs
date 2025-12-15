using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TargetingSystem
{
    //It can be eyesight, or radar 
    public class TargetTracker
    {
        public event Action<ITargetable> OnTargetAdded;

        public event Action<ITargetable> OnTargetRemoved;

        protected List<ITargetable> targets = new List<ITargetable>();

        public void AddTarget(ITargetable target)
        {
            targets.Add(target);
            OnTargetAdded?.Invoke(target);
        }

        public void RemoveTarget(ITargetable target)
        {
            targets.Remove(target);
            OnTargetRemoved?.Invoke(target);
        }
    }
}
