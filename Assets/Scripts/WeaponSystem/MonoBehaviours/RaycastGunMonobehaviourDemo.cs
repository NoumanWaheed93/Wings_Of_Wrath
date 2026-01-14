using UnityEngine;
using Common;

namespace WeaponSystem
{
    public class RaycastGunMonobehaviourDemo : WeaponMonoBehaviour
    {
        [SerializeField]
        private float range;

        [SerializeField]
        private float damageAmount;

        private void Awake()
        {
            weapon = new GunRaycastBased(transform, new GameTimeProvider(), maxAmmo, bulletsPerSecond, range, damageAmount);
        }

        public void Fire()
        {
            Debug.Log("Firing Raycast Gun");
            weapon.Fire();
        }
    }
}
