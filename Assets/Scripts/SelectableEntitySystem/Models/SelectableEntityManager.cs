using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SelectableEntitySystem
{
    public class SelectableEntityManager
    {
        public event Action<SelectableEntity> OnSelectableEntityAdded;
        public event Action<SelectableEntity> OnSelectableEntityRemoved;

        private List<SelectableEntity> selectableEntities = new List<SelectableEntity>();

        private SelectableEntity selectedEntity;

        public void AddSelectableEntity(SelectableEntity entity)
        {
            selectableEntities.Add(entity);
            OnSelectableEntityAdded?.Invoke(entity);
        }

        public void RemoveSelectableEntity(SelectableEntity entity)
        {
            selectableEntities.Remove(entity);
            OnSelectableEntityRemoved?.Invoke(entity);
        }

        public void SelectEntity(SelectableEntity entity)
        {
            if (selectedEntity != null)
            {
                selectedEntity.Deselect();
            }

            if (selectableEntities.Contains(entity))
            {
                entity.Select();
                selectedEntity = entity;
            }
        }
    }

}
