using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommandSystem;

namespace CommandSystem.UI
{
    public class UI_CommandButton : MonoBehaviour
    {
        public event Action<ICommandDefinition> OnClicked;

        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI buttonText;

        private ICommandDefinition commandDefinition;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void Init(ICommandDefinition commandDefinition)
        {
            this.commandDefinition = commandDefinition;
            buttonText.text = commandDefinition.Name;
        }

        private void OnClick()
        {
            OnClicked?.Invoke(commandDefinition);
        }
    }

}
