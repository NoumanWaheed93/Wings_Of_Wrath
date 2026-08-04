namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when this aircraft holds the first position of the formation, which means it leads it
    /// and therefore flies the route instead of following anyone.
    /// </summary>
    public class TransitionOnBecameFormationLeader : AITransition
    {
        public TransitionOnBecameFormationLeader(AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
        }

        public override bool IsTriggered()
        {
            return aircraftController.FormationMember.PositionIndex == 0;
        }
    }
}
