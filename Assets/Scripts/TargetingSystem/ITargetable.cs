using Common;

namespace TargetingSystem
{
    public interface ITargetable
    {
        public Team Team { get; }

        public ITransform Transform { get; }
    }
}
