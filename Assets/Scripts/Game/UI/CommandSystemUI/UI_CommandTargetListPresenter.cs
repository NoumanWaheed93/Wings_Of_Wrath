using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TargetingSystem;
using Game.Commands;

namespace Game.UI.CommandSystemUI
{
    /// <summary>
    /// Step 3 of the command demo: lists every ITargetable in the CommandTargetManager.
    /// Only shown for commands whose ICommandDefinition.RequiresTarget is true.
    /// </summary>
    public class UI_CommandTargetListPresenter : MonoBehaviour
    {
        public event Action<ITargetable> OnTargetChosen;

        [SerializeField]
        private Transform targetButtonsParent;
        [SerializeField]
        private UI_CommandTargetButton targetButtonPrefab;

        private CommandTargetManager commandTargetManager;

        private readonly Dictionary<ITargetable, UI_CommandTargetButton> targetToButton
            = new Dictionary<ITargetable, UI_CommandTargetButton>();

        [Inject]
        public void Init(CommandTargetManager commandTargetManager)
        {
            this.commandTargetManager = commandTargetManager;
        }

        private void Awake()
        {
            commandTargetManager.OnTargetAdded += CommandTargetManager_OnTargetAdded;
            commandTargetManager.OnTargetRemoved += CommandTargetManager_OnTargetRemoved;
        }

        private void CommandTargetManager_OnTargetAdded(ITargetable target)
        {
            UI_CommandTargetButton button = Instantiate(targetButtonPrefab, targetButtonsParent);
            button.Init(target);
            button.OnClicked += Button_OnClicked;
            targetToButton.Add(target, button);
        }

        private void CommandTargetManager_OnTargetRemoved(ITargetable target)
        {
            if (targetToButton.TryGetValue(target, out UI_CommandTargetButton button))
            {
                button.OnClicked -= Button_OnClicked;
                Destroy(button.gameObject);
                targetToButton.Remove(target);
            }
        }

        private void Button_OnClicked(ITargetable target)
        {
            OnTargetChosen?.Invoke(target);
        }
    }

}
