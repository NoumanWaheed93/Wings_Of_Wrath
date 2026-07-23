namespace AircraftController
{
    public interface IAircraftController
    {
        public bool IsActive { get; set; }
        public bool IsAfterBurnerOn { get; }
        public float AltitudeOffset { get; }
        public float GetDesiredSpeed();
        public float GetTurn();
        public void Update(float simulationDeltaTime);
    }
}
