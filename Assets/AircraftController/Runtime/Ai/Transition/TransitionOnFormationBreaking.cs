namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when this aircraft has been ordered to break away from its formation.
    /// </summary>
    public class TransitionOnFormationBreaking : AITransition
    {
        public TransitionOnFormationBreaking(AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
        }

        public override bool IsTriggered()
        {
            return aircraftController.IsFormationBreaking;
        }
    }
}
