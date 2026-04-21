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

        public event Action<SelectableEntity> OnEntitySelected;

        private List<SelectableEntity> selectableEntities = new List<SelectableEntity>();

        private SelectableEntity selectedEntity;

        public IReadOnlyList<SelectableEntity> SelectableEntities => selectableEntities.AsReadOnly();

        public void AddSelectableEntity(SelectableEntity entity)
        {
            Debug.Log($"AddSelectableEntity({entity.Name})");

            selectableEntities.Add(entity);
            OnSelectableEntityAdded?.Invoke(entity);
        }

        public void RemoveSelectableEntity(SelectableEntity entity)
        {
            Debug.Log($"RemoveSelectableEntity({entity.Name})");

            selectableEntities.Remove(entity);
            OnSelectableEntityRemoved?.Invoke(entity);
        }

        public void SelectEntity(SelectableEntity entity)
        {
            Debug.Log($"SelectEntity({entity.Name})");

            if (selectedEntity != null)
            {
                selectedEntity.Deselect();
            }

            if (selectableEntities.Contains(entity))
            {
                Debug.Log("Succesfully selected entity");

                entity.Select();
                selectedEntity = entity;
                OnEntitySelected?.Invoke(entity);
            }
        }

    }

}
