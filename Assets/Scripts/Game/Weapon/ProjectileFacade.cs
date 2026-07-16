using UnityEngine;
using HealthSystem;
using Zenject;

namespace Game
{
    public class ProjectileFacade : MonoBehaviour
    {
        private IMemoryPool pool;

        public Transform Transform => transform;

        protected virtual void Awake()
        {
            GetComponent<CollisionDamager>().OnCollided += OnCollided;
        }

        public void SetPool(IMemoryPool pool)
        {
            this.pool = pool;
        }

        private void OnCollided()
        {
            pool.Despawn(this);
        }
    }
}
