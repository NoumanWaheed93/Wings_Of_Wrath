using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace SelectableEntitySystem
{
    public class UI_SelectableEntityManagerPresenter : MonoBehaviour
    {
        [SerializeField]
        private Transform entitySelectionButtonsParent;
        [SerializeField]
        private UI_SelectableEntitySelectButton entitySelectionButtonPrefab;

        private SelectableEntityManager selectableEntityManager;

        private Dictionary<SelectableEntity, UI_SelectableEntitySelectButton> entityToButtonDictionary 
            = new Dictionary<SelectableEntity, UI_SelectableEntitySelectButton>();

        private void Awake()
        {
            selectableEntityManager.OnSelectableEntityAdded += SelectableEntityManager_OnSelectableEntityAdded;
            selectableEntityManager.OnSelectableEntityRemoved += SelectableEntityManager_OnSelectableEntityRemoved;
        }

        [Inject]
        public void Init(SelectableEntityManager selectableEntityManager)
        {
            this.selectableEntityManager = selectableEntityManager;
        }

        private void SelectableEntityManager_OnSelectableEntityAdded(SelectableEntity obj)
        {
            UI_SelectableEntitySelectButton button = Instantiate(entitySelectionButtonPrefab, entitySelectionButtonsParent);
            button.Init(obj);
            button.OnSelected += Button_OnSelected;
            entityToButtonDictionary.Add(obj, button);
        }

        private void Button_OnSelected(SelectableEntity obj)
        {
            selectableEntityManager.SelectEntity(obj);
        }

        private void SelectableEntityManager_OnSelectableEntityRemoved(SelectableEntity obj)
        {
            if (entityToButtonDictionary.TryGetValue(obj, out UI_SelectableEntitySelectButton button))
            {
                Destroy(button.gameObject);
                entityToButtonDictionary.Remove(obj);
            }
        }


    }

}
