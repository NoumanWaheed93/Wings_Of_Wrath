using System;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    /// <summary>
    /// Listens to the formation command keys and forwards them to the FormationCommandSystem.
    /// </summary>
    public class FormationCommandInput : IInitializable, IDisposable
    {
        private InputAction joinFormationAction;
        private InputAction breakFormationAction;
        private FormationCommandSystem formationCommandSystem;

        public FormationCommandInput(InputAction joinFormationAction, InputAction breakFormationAction, FormationCommandSystem formationCommandSystem)
        {
            this.joinFormationAction = joinFormationAction;
            this.breakFormationAction = breakFormationAction;
            this.formationCommandSystem = formationCommandSystem;
        }

        public void Initialize()
        {
            joinFormationAction.performed += OnJoinFormationPerformed;
            breakFormationAction.performed += OnBreakFormationPerformed;
            joinFormationAction.Enable();
            breakFormationAction.Enable();
        }

        public void Dispose()
        {
            joinFormationAction.performed -= OnJoinFormationPerformed;
            breakFormationAction.performed -= OnBreakFormationPerformed;
            joinFormationAction.Disable();
            breakFormationAction.Disable();
        }

        private void OnJoinFormationPerformed(InputAction.CallbackContext context)
        {
            formationCommandSystem.CommandJoinFormation();
        }

        private void OnBreakFormationPerformed(InputAction.CallbackContext context)
        {
            formationCommandSystem.CommandBreakFormation();
        }

    }

}
