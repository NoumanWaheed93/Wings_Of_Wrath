using System;
using System.Collections.Generic;

namespace CommandSystem
{
    public class CommandManager
    {
        public event Action<ICommandable> OnCommandableAdded;
        public event Action<ICommandable> OnCommandableRemoved;
        public event Action<ICommandable> OnCommandableSelected;
        public event Action<ICommandable> OnCommandableDeselected;

        private readonly HashSet<ICommandable> commandables = new HashSet<ICommandable>();
        private readonly HashSet<ICommandable> selectedCommandables = new HashSet<ICommandable>();

        public IReadOnlyCollection<ICommandable> Commandables => commandables;
        public IReadOnlyCollection<ICommandable> SelectedCommandables => selectedCommandables;

        public void AddCommandable(ICommandable commandable)
        {
            commandables.Add(commandable);
            OnCommandableAdded?.Invoke(commandable);
        }

        public void RemoveCommandable(ICommandable commandable)
        {
            commandables.Remove(commandable);
            if (selectedCommandables.Remove(commandable))
            {
                OnCommandableDeselected?.Invoke(commandable);
            }
            OnCommandableRemoved?.Invoke(commandable);
        }

        public bool ToggleCommandableSelection(ICommandable commandable)
        {
            if (selectedCommandables.Add(commandable))
            {
                OnCommandableSelected?.Invoke(commandable);
                return true;
            }

            selectedCommandables.Remove(commandable);
            OnCommandableDeselected?.Invoke(commandable);
            return false;
        }

        public bool ToggleAllCommandableSelection()
        {
            bool selectAll = selectedCommandables.Count < commandables.Count;

            foreach (ICommandable commandable in commandables)
            {
                if (selectAll)
                {
                    if (selectedCommandables.Add(commandable))
                    {
                        OnCommandableSelected?.Invoke(commandable);
                    }
                }
                else
                {
                    if (selectedCommandables.Remove(commandable))
                    {
                        OnCommandableDeselected?.Invoke(commandable);
                    }
                }
            }

            return selectAll;
        }

        public void GiveCommandToSelected(ICommand command)
        {
            foreach (ICommandable commandable in selectedCommandables)
            {
                commandable.GiveCommand(command);
            }
        }
    }

}
