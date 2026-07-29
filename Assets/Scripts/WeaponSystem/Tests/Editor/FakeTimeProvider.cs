using Common;

/// <summary>
/// An ITimeProvider for tests. Real time is ignored: the clock only moves
/// when a test tells it to move, so time based tests are fast and repeatable.
/// </summary>
public class FakeTimeProvider : ITimeProvider
{
    private float currentTime;

    public float CurrentTime { get => currentTime; set => currentTime = value; }

    public FakeTimeProvider(float startTime = 0f)
    {
        currentTime = startTime;
    }

    public float GetTime()
    {
        return currentTime;
    }

    /// <summary>
    /// Moves the clock forward by the given number of seconds.
    /// </summary>
    public void Advance(float seconds)
    {
        currentTime += seconds;
    }
}
