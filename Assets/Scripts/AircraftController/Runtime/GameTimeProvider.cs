using UnityEngine;

namespace AircraftController
{
    public class GameTimeProvider : ITimeProvider
    {
        public float GetTime()
        {
            return Time.time;
        }
    }
}
