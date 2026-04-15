using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectableEntitySystem
{
    public class SelectableEntityManagerUI
    {
        private SelectableEntityManager SelectableEntityManager;

        public SelectableEntityManagerUI(SelectableEntityManager selectableEntityManager)
        {
            this.SelectableEntityManager = selectableEntityManager;
            selectableEntityManager.OnSelectableEntityAdded += SelectableEntityManager_OnSelectableEntityAdded;
            selectableEntityManager.OnSelectableEntityRemoved += SelectableEntityManager_OnSelectableEntityRemoved;
        }

        private void SelectableEntityManager_OnSelectableEntityAdded(SelectableEntity obj)
        {
            throw new NotImplementedException();
        }
        private void SelectableEntityManager_OnSelectableEntityRemoved(SelectableEntity obj)
        {
            throw new NotImplementedException();
        }

    }

}
