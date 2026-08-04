namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when the break away manoeuvre is done, either because the break destination has been
    /// reached or because the aircraft is not breaking anymore.
    /// </summary>
    public class TransitionOnFormationBreakFinished : AITransition
    {
        private StateBreakFormation breakFormationState;

        public TransitionOnFormationBreakFinished(StateBreakFormation breakFormationState, AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
            this.breakFormationState = breakFormationState;
        }

        public override bool IsTriggered()
        {
            return breakFormationState.HasReachedBreakDestination
                || aircraftController.IsFormationBreaking == false;
        }
    }
}
