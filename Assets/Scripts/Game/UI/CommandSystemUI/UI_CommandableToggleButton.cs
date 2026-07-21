using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommandSystem;

namespace Game.UI.CommandSystemUI
{
    public class UI_CommandableToggleButton : MonoBehaviour
    {
        private static readonly Color SelectedColor = Color.green;
        private static readonly Color DeselectedColor = Color.white;

        public event Action<ICommandable> OnClicked;

        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI buttonText;

        private ICommandable commandable;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void Init(ICommandable commandable)
        {
            this.commandable = commandable;
            buttonText.text = commandable.Name;
            SetToggled(false);
        }

        public void SetToggled(bool isOn)
        {
            button.targetGraphic.color = isOn ? SelectedColor : DeselectedColor;
        }

        private void OnClick()
        {
            OnClicked?.Invoke(commandable);
        }
    }

}
