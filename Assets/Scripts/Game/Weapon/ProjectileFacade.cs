using UnityEngine;
using HealthSystem;
using Zenject;

namespace Game
{
    public class ProjectileFacade : MonoBehaviour
    {
        private IMemoryPool pool;
        private Rigidbody m_rigidbody;
        private bool isActive;

        public Transform Transform => transform;

        protected virtual void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            GetComponent<CollisionDamager>().OnCollided += OnCollided;
        }

        public void SetPool(IMemoryPool pool)
        {
            this.pool = pool;
        }

        public void ResetPhysics()
        {
            m_rigidbody.linearVelocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            isActive = true;
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
    }
}
