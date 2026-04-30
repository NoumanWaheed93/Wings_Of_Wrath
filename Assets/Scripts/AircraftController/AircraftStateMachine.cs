namespace AircraftController
{
    public class AircraftStateMachine
    {
        public AircraftState currentState { get; private set; }

        public void ChangeState(AircraftState newState)
        {
            currentState?.Exit();

            currentState = newState;
            currentState.Enter();
        }
    
    }

}
