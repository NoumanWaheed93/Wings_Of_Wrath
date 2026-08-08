namespace AircraftController
{
    namespace AircraftAI
    {
        /// <summary>
        /// A transition owns the condition that leads out of one state into another.
        /// States do not decide where to go next, so they never need to know about each other.
        /// Transitions are registered on the <see cref="AIStateMachine"/> per source state.
        /// </summary>
        public abstract class AITransition
        {
            protected AircraftAIController aircraftController;

            /// <summary>
            /// The state to enter when this transition is triggered.
            /// </summary>
            public AIState To { get; private set; }

            public AITransition(AIState to, AircraftAIController aircraftController)
            {
                this.To = to;
                this.aircraftController = aircraftController;
            }

            /// <summary>
            /// Returns true when the state machine should leave the source state and enter <see cref="To"/>.
            /// </summary>
            public abstract bool IsTriggered();
        }
    }
}
