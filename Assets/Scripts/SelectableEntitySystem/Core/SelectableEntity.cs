using System;

namespace SelectableEntitySystem
{
    public class SelectableEntity
    {
        public event Action<SelectableEntity> OnSelect;
        public event Action<SelectableEntity> OnDeselect;

        public void Select() 
        {
            OnSelect?.Invoke(this);
        }

        public void DeSelect()
        {
            OnDeselect?.Invoke(this);
        }
    }

}
