using UnityEngine;
using Zenject;
using CommandSystem;
using TargetingSystem;

namespace Game.UI.CommandSystemUI
{
    /// <summary>
    /// Drives the 3-step command demo flow: select commandables -> select a command -> select
    /// a command target (only when the chosen command requires one).
    /// </summary>
    public class UI_CommandFlowPresenter : MonoBehaviour
    {
        private enum Step
        {
            SelectCommandables,
            SelectCommand,
            SelectTarget
        }

        [SerializeField]
        private GameObject commandableStepPanel;
        [SerializeField]
        private GameObject commandStepPanel;
        [SerializeField]
        private GameObject targetStepPanel;

        [SerializeField]
        private UI_CommandableListPresenter commandableListPresenter;
        [SerializeField]
        private UI_CommandPalettePresenter commandPalettePresenter;
        [SerializeField]
        private UI_CommandTargetListPresenter commandTargetListPresenter;

        private CommandManager commandManager;

        private ICommandDefinition pendingCommandDefinition;

        [Inject]
        public void Init(CommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        private void Awake()
        {
            commandableListPresenter.OnGiveCommand += CommandableListPresenter_OnGiveCommand;
            commandPalettePresenter.OnCommandChosen += CommandPalettePresenter_OnCommandChosen;
            commandTargetListPresenter.OnTargetChosen += CommandTargetListPresenter_OnTargetChosen;
        }

        private void Start()
        {
            // Deferred to Start(): every other panel's Awake() (button setup, event subscriptions)
            // is guaranteed to have already run by this point, so it's safe to deactivate them here.
            SetStep(Step.SelectCommandables);
        }

        private void CommandableListPresenter_OnGiveCommand()
        {
            SetStep(Step.SelectCommand);
        }

        private void CommandPalettePresenter_OnCommandChosen(ICommandDefinition definition)
        {
            if (definition.RequiresTarget)
            {
                pendingCommandDefinition = definition;
                SetStep(Step.SelectTarget);
                return;
            }

            commandManager.GiveCommandToSelected(definition.CreateCommand(null));
            SetStep(Step.SelectCommandables);
        }

        private void CommandTargetListPresenter_OnTargetChosen(ITargetable target)
        {
            commandManager.GiveCommandToSelected(pendingCommandDefinition.CreateCommand((ICommandTarget)target));
            pendingCommandDefinition = null;
            SetStep(Step.SelectCommandables);
        }

        private void SetStep(Step step)
        {
            commandableStepPanel.SetActive(step == Step.SelectCommandables);
            commandStepPanel.SetActive(step == Step.SelectCommand);
            targetStepPanel.SetActive(step == Step.SelectTarget);
        }
    }

}
