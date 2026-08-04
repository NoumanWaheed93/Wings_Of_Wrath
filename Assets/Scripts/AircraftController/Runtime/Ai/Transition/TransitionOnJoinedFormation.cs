namespace AircraftController.AircraftAI
{
    /// <summary>
    /// Triggered when this aircraft is a wingman of a formation that it is not breaking away from.
    /// </summary>
    public class TransitionOnJoinedFormation : AITransition
    {
        public TransitionOnJoinedFormation(AIState to, AircraftAIController aircraftController) : base(to, aircraftController)
        {
        }

        public override bool IsTriggered()
        {
            AircraftFormationMember formationMember = aircraftController.FormationMember;
            return formationMember.Formation != null
                && formationMember.PositionIndex != 0
                && aircraftController.IsFormationBreaking == false;
        }
    }
}
