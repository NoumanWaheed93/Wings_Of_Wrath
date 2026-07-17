using UnityEngine;
using HealthSystem;
using Zenject;
using WeaponSystem;

namespace Game
{
    public class ProjectileFacade : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private TrailRenderer trail;

        private Projectile projectile;
        private IMemoryPool pool;
        private bool isActive;
        
        public Transform Transform => transform;

        protected virtual void Awake()
        {
            projectile = GetComponent<Projectile>();
            GetComponent<CollisionDamager>().OnCollided += OnCollided;
        }

        public void SetPool(IMemoryPool pool)
        {
            this.pool = pool;
        }

        private void OnCollided()
        {
            // A single impact can raise this event more than once (e.g. the missile's collision
            // and a trigger overlap both firing), so guard against despawning the same item twice.
            if (!isActive)
                return;

            isActive = false;
            pool.Despawn(this);
        }

        public void Launch(Transform barrel)
        {
            isActive = true;
            projectile.Launch(barrel);
            trail.Clear();
        }
    }
}
