using UnityEngine;

namespace WeaponSystem
{
    public interface IGuidedProjectile
    {
        public Transform Transform { get; }
        public Transform Target { get; set; }
    }
}
