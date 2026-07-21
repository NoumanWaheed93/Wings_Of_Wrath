using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TargetingSystem;

namespace Game.UI.CommandSystemUI
{
    public class UI_CommandTargetButton : MonoBehaviour
    {
        public event Action<ITargetable> OnClicked;

        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI buttonText;

        private ITargetable target;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void Init(ITargetable target)
        {
            this.target = target;
            buttonText.text = target.Transform.name;
        }

        private void OnClick()
        {
            OnClicked?.Invoke(target);
        }
    }

}
