namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when this aircraft itself has reached the final approach, so the AI has to fly the landing.
    /// </summary>
    public class TransitionOnFinalApproach : AITransition
    {
        public TransitionOnFinalApproach(AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
        }

        public override bool IsTriggered()
        {
            IAircraft aircraft = aircraftController.aircraft;
            return aircraft.StateMachine.currentState == aircraft.StateFinalApproach;
        }
    }
}
