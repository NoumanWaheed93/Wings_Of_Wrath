using UnityEngine;
using Common;

namespace WeaponSystem
{
    public class RaycastGunMonobehaviourDemo : WeaponMonoBehaviour
    {
        [SerializeField]
        private float range;

        private void Awake()
        {
            weapon = new GunRaycastBased(transform, new GameTimeProvider(), maxAmmo, bulletsPerSecond, range);
        }
    }
}
