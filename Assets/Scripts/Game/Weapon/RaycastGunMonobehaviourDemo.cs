using UnityEngine;
using WeaponSystem;
using HealthSystem;

namespace Game
{
    public class RaycastGunMonobehaviourDemo : WeaponMonoBehaviour
    {
        [SerializeField]
        private float range;

        [SerializeField]
        private float damageAmount;

        [SerializeField]
        private GameObject bulletLine;

        private float bulletLife = 0;

        private class GunRaycastDemo : GunRaycastBased
        {
            public GunRaycastDemo(Transform barrel, ITimeProvider timeProvider, int maximumAmmo, float bulletsPerSecond, float range, float damageAmount) : base(barrel, timeProvider, maximumAmmo, bulletsPerSecond, range, damageAmount)
            {
            }

            public override void ApplyDamage(RaycastHit hitInfo, float damageAmount)
            {
                if(hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    Debug.Log($"Raycast hit {hitInfo.collider.name} at distance {hitInfo.distance}, applying {damageAmount} damage.");
                    damageable.Damage(damageAmount);
                }
            }
        }

        private void Awake()
        {
            weapon = new GunRaycastDemo(transform, new GameTimeProvider(), maxAmmo, bulletsPerSecond, range, damageAmount);
        }

        private void Update()
        {
            bulletLife -= Time.deltaTime;
            if (bulletLife <= 0)
            {
                bulletLine.SetActive(false);
            }
        }

        public void Fire()
        {
            Debug.Log("Firing Raycast Gun");
            if (weapon.Fire())
            {
                bulletLife = 0.035f;
                bulletLine.SetActive(true);
            }
        }
    }
}
