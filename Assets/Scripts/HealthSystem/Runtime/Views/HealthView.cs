using UnityEngine;

namespace HealthSystem
{
    public abstract class HealthView : MonoBehaviour
    {
        private Health health;
        protected Health CurrentHealth { get { return health; } }

        public Health Health
        {
            get { return health; }
            set { Init(value); }
        }

        public void Init(Health health)
        {
            UnSubscribe();
            this.health = health;
            Subscribe();
            Refresh();
        }

        protected virtual void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            if (health == null)
                return;

            health.onHealthReduced += Refresh;
            health.onHealthGained += Refresh;
        }

        private void UnSubscribe()
        {
            if (health == null)
                return;

            health.onHealthReduced -= Refresh;
            health.onHealthGained -= Refresh;
        }

        protected abstract void Refresh();
    }
}
