using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommandSystem;

namespace CommandSystem.UI
{
    public class UI_CommandTargetButton : MonoBehaviour
    {
        public event Action<ICommandTarget> OnClicked;

        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI buttonText;

        private ICommandTarget target;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void Init(ICommandTarget target)
        {
            this.target = target;
            buttonText.text = target.Name;
        }

        private void OnClick()
        {
            OnClicked?.Invoke(target);
        }
    }

}
