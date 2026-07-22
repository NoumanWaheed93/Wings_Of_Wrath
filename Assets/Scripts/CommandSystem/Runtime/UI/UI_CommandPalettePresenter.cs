using System;
using UnityEngine;
using Zenject;
using CommandSystem;

namespace CommandSystem.UI
{
    /// <summary>
    /// Step 2 of the command demo: lists every ICommandDefinition in the CommandPalette.
    /// </summary>
    public class UI_CommandPalettePresenter : MonoBehaviour
    {
        public event Action<ICommandDefinition> OnCommandChosen;

        [SerializeField]
        private Transform commandButtonsParent;
        [SerializeField]
        private UI_CommandButton commandButtonPrefab;

        private ICommandPallette commandPalette;

        [Inject]
        public void Init(ICommandPallette commandPalette)
        {
            this.commandPalette = commandPalette;
        }

        private void Awake()
        {
            foreach (ICommandDefinition definition in commandPalette.Definitions)
            {
                UI_CommandButton button = Instantiate(commandButtonPrefab, commandButtonsParent);
                button.Init(definition);
                button.OnClicked += Button_OnClicked;
            }
        }

        private void Button_OnClicked(ICommandDefinition definition)
        {
            OnCommandChosen?.Invoke(definition);
        }
    }

}
