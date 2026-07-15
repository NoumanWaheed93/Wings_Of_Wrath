using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace TargetingSystem
{
    //It can be eyesight, or radar 
    public class TargetTracker
    {
        public event Action<ITargetable> OnAddedTarget;
        public event Action<ITargetable> OnRemovedTarget;

        public event Action<ITargetable> OnDeselectTarget;
        public event Action<ITargetable> OnSelectTarget;

        protected List<ITargetable> targets = new List<ITargetable>();
        public ReadOnlyCollection<ITargetable> TargetsList { get => targets.AsReadOnly(); }

        private ITargetable selectedTarget;
        public ITargetable SelectedTarget 
        { 
            get 
            { 
                return selectedTarget; 
            }
            set
            {
                if (selectedTarget != value)
                {
                    if (selectedTarget != null)
                    {
                        OnDeselectTarget(selectedTarget);
                    }
                    selectedTarget = value;
                    OnSelectTarget(selectedTarget);
                }
            }
        }

        public void AddTarget(ITargetable target)
        {
            targets.Add(target);
            OnAddedTarget?.Invoke(target);
        }

        public void RemoveTarget(ITargetable target)
        {
            targets.Remove(target);
            OnRemovedTarget?.Invoke(target);
        }
    }
}
