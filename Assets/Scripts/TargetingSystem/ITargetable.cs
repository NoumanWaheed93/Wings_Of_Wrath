using Common;
using UnityEngine;

namespace TargetingSystem
{
    public interface ITargetable
    {
        public Team Team { get; }

        public Transform Transform { get; }
    }
}
