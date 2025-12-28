using UnityEngine;

namespace WeaponSystem
{
    public interface IHomingProjectile
    {
        public Transform Transform { get; }
        public Transform Target { get; set; }
    }
}
