using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SelectableEntitySystem
{
    public class UI_SelectableEntitySelectButton : MonoBehaviour
    {
        public event Action<SelectableEntity> OnSelected;

        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI buttonText;

        private SelectableEntity selectableEntity;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);

            selectableEntity.OnSelect += SelectableEntity_OnSelect;
            selectableEntity.OnDeselect += SelectableEntity_OnDeselect;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
            if (selectableEntity != null)
            {
                selectableEntity.OnSelect -= SelectableEntity_OnSelect;
                selectableEntity.OnDeselect -= SelectableEntity_OnDeselect;
            }
        }

        public void Init(SelectableEntity selectableEntity)
        {
            this.selectableEntity = selectableEntity;
            buttonText.text = selectableEntity.Name;
        }

        public void OnClick()
        {
            OnSelected?.Invoke(selectableEntity);
        }

        public void SelectableEntity_OnSelect(SelectableEntity entity)
        {
            button.interactable = false;
        }

        public void SelectableEntity_OnDeselect(SelectableEntity entity)
        {
            button.interactable = true;
        }

    }

}