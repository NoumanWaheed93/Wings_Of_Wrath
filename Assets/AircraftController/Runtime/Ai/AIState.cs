namespace AircraftController
{
    namespace AircraftAI
    {
        /// <summary>
        /// A state only describes what the aircraft does while it is active.
        /// Leaving the state is the job of an <see cref="AITransition"/>, which is why a state
        /// has no reference to the state machine or to any other state.
        /// </summary>
        public abstract class AIState
        {
            protected AircraftAIController aircraftController;

            public AIState(AircraftAIController aircraftController)
            {
                this.aircraftController = aircraftController;
            }

            public abstract void Enter();

            public abstract void Update(float simulationDeltaTime);

            public abstract void Exit();
        }
    }
}
