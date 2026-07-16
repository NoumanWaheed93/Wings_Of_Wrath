using FormationSystem;
using UnityEngine;
using AircraftController.AircraftAI;

namespace AircraftController
{
    public class AircraftFormationMember : IFormationMember<AircraftFormationMember>
    {
        public AircraftFormationMember Self { get => this; }

        public int PositionIndex { get; set; }

        public Vector3 Position { get; set; }

        public Formation<AircraftFormationMember> Formation { get; set; }

        public IAircraft aircraft;

        public AircraftAIController aiController;

    }

}
