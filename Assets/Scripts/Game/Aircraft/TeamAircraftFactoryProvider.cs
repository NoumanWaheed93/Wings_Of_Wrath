using System.Collections.Generic;
using Common;
using Zenject;

namespace Game
{
    public class TeamAircraftFactoryProvider
    {
        private readonly Dictionary<Team, AircraftFacade.Pool> pools;

        public TeamAircraftFactoryProvider(
            [Inject(Id = Team.Blue)] AircraftFacade.Pool bluePool,
            [Inject(Id = Team.Red)] AircraftFacade.Pool redPool)
        {
            pools = new Dictionary<Team, AircraftFacade.Pool>
            {
                { Team.Blue, bluePool },
                { Team.Red, redPool }
            };
        }

        public AircraftFacade.Pool GetPool(Team team) => pools[team];
    }
}
