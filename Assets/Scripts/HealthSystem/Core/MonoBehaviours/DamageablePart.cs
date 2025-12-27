using UnityEngine;

namespace HealthSystem
{
    public class DamageablePart : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private float damageMultiplier;

        private DamageablePartController controller;

        public void Init(Health health)
        {
            controller = new DamageablePartController(health, damageMultiplier);
        }

        public void Damage(float amount)
        {
            controller.Damage(amount);
        }
    }
}
