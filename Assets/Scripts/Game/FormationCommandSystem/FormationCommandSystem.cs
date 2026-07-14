using System.Collections.Generic;
using AircraftController.AircraftAI;

namespace Game
{
    /// <summary>
    /// Receives user commands for the formation and relays them to the AI controllers of its members.
    /// The AI states react to the flags set here, so the decision making stays inside the states.
    /// </summary>
    public class FormationCommandSystem
    {
        private List<AircraftAIController> aiControllers = new List<AircraftAIController>();

        public void Register(AircraftAIController aiController)
        {
            if (aiControllers.Contains(aiController) == false)
            {
                aiControllers.Add(aiController);
            }
        }

        public void Unregister(AircraftAIController aiController)
        {
            aiControllers.Remove(aiController);
        }

        public void CommandJoinFormation()
        {
            SetFormationBreaking(false);
        }

        public void CommandBreakFormation()
        {
            SetFormationBreaking(true);
        }

        private void SetFormationBreaking(bool isBreaking)
        {
            foreach (AircraftAIController aiController in aiControllers)
            {
                if (aiController.FormationMember.Formation == null)
                {
                    continue;
                }

                aiController.IsFormationBreaking = isBreaking;
            }
        }

    }

}
