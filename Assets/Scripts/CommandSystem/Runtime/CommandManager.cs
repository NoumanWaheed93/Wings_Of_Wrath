using System.Collections.Generic;
using System.Linq;

namespace CommandSystem
{
    public class CommandManager
    {
        /// <summary>
        /// This dicitionary contains the list of all commandables, and the boolean indicates
        /// whether the commandable is selected for the command or not.
        /// </summary>
        private Dictionary<ICommandable, bool> dictSelectedCommandables = new Dictionary<ICommandable, bool>();
        
        public void AddCommandable(ICommandable commandable)
        {
            dictSelectedCommandables[commandable] = false; //by default a new commandable is not selected
        }

        public void RemoveCommandable(ICommandable commandable)
        {
            dictSelectedCommandables.Remove(commandable);
        }

        public bool ToggleCommandableSelection(ICommandable commandable)
        {
            bool isSelected = !dictSelectedCommandables[commandable];
            dictSelectedCommandables[commandable] = isSelected;
            return isSelected;
        }

        public bool ToggleAllCommandableSelection()
        {
            bool mostCommon = dictSelectedCommandables.Values
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count()).First().Key;

            bool isSelected = !mostCommon;
            foreach(ICommandable commandable in dictSelectedCommandables.Keys)
            {
                dictSelectedCommandables[commandable] = isSelected;
            }
            return isSelected;
        }

        public void GiveCommandToSelected(ICommand command)
        {
            foreach(ICommandable commandable in dictSelectedCommandables.Keys)
            {
                commandable.GiveCommand(command);
            }
        }
    }

}
