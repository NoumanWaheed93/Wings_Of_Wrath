namespace AircraftController
{
    public interface IAircraftPlayerInputManager
    {
        float SteerDirection { get; }
        bool IsAfterBurnerOn { get; }
    }

}
