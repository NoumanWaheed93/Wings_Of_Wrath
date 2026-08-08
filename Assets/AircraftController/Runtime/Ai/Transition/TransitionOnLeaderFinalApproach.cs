namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when the formation leader has reached the final approach, so the whole formation lands.
    /// </summary>
    public class TransitionOnLeaderFinalApproach : AITransition
    {
        public TransitionOnLeaderFinalApproach(AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
        }

        public override bool IsTriggered()
        {
            AircraftFormationMember formationMember = aircraftController.FormationMember;
            if (formationMember.Formation == null || formationMember.Formation.leader == null)
            {
                return false;
            }

            IAircraft leader = formationMember.Formation.leader.aircraft;
            return leader.StateMachine.currentState == leader.StateFinalApproach;
        }
    }
}
