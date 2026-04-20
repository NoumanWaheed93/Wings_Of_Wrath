using System;

namespace SelectableEntitySystem
{
    public class SelectableEntity
    {
        public event Action<SelectableEntity> OnSelect;
        public event Action<SelectableEntity> OnDeselect;

        public string Name { get; private set; }

        public SelectableEntity(string name)
        {
            this.Name = name;
        }

        public void Select() 
        {
            OnSelect?.Invoke(this);
        }

        public void Deselect()
        {
            OnDeselect?.Invoke(this);
        }

    }

}
