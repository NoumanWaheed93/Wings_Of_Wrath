using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using CommandSystem;

namespace CommandSystem.UI
{
    /// <summary>
    /// Step 1 of the command demo: lists every registered ICommandable as a toggle button
    /// (multi-select, unlike the single-select UI_SelectableEntitySelectButton this mirrors).
    /// </summary>
    public class UI_CommandableListPresenter : MonoBehaviour
    {
        public event Action OnGiveCommand;

        [SerializeField]
        private Transform commandableButtonsParent;
        [SerializeField]
        private UI_CommandableToggleButton commandableButtonPrefab;
        [SerializeField]
        private Button selectAllButton;
        [SerializeField]
        private Button giveCommandButton;

        private CommandManager commandManager;

        private readonly Dictionary<ICommandable, UI_CommandableToggleButton> commandableToButton
            = new Dictionary<ICommandable, UI_CommandableToggleButton>();

        [Inject]
        public void Init(CommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        private void Awake()
        {
            commandManager.OnCommandableAdded += CommandManager_OnCommandableAdded;
            commandManager.OnCommandableRemoved += CommandManager_OnCommandableRemoved;
            commandManager.OnCommandableSelected += CommandManager_OnCommandableSelected;
            commandManager.OnCommandableDeselected += CommandManager_OnCommandableDeselected;

            selectAllButton.onClick.AddListener(OnClick_SelectAll);
            giveCommandButton.onClick.AddListener(OnClick_GiveCommand);
            RefreshGiveCommandInteractable();
        }

        private void CommandManager_OnCommandableAdded(ICommandable commandable)
        {
            UI_CommandableToggleButton button = Instantiate(commandableButtonPrefab, commandableButtonsParent);
            button.Init(commandable);
            button.OnClicked += Button_OnClicked;
            commandableToButton.Add(commandable, button);
        }

        private void CommandManager_OnCommandableRemoved(ICommandable commandable)
        {
            if (commandableToButton.TryGetValue(commandable, out UI_CommandableToggleButton button))
            {
                button.OnClicked -= Button_OnClicked;
                Destroy(button.gameObject);
                commandableToButton.Remove(commandable);
            }
        }

        private void CommandManager_OnCommandableSelected(ICommandable commandable)
        {
            if (commandableToButton.TryGetValue(commandable, out UI_CommandableToggleButton button))
            {
                button.SetToggled(true);
            }
            RefreshGiveCommandInteractable();
        }

        private void CommandManager_OnCommandableDeselected(ICommandable commandable)
        {
            if (commandableToButton.TryGetValue(commandable, out UI_CommandableToggleButton button))
            {
                button.SetToggled(false);
            }
            RefreshGiveCommandInteractable();
        }

        private void Button_OnClicked(ICommandable commandable)
        {
            commandManager.ToggleCommandableSelection(commandable);
        }

        private void OnClick_SelectAll()
        {
            commandManager.ToggleAllCommandableSelection();
        }

        private void OnClick_GiveCommand()
        {
            OnGiveCommand?.Invoke();
        }

        private void RefreshGiveCommandInteractable()
        {
            giveCommandButton.interactable = commandManager.SelectedCommandables.Count > 0;
        }
    }

}
