using UnityEngine;

namespace HealthSystem
{
    public class DamageablePart : MonoBehaviour, IDamageable
    {
        private const string LOG_FORMAT = "<color=#00FF62><b>[DamageablePart]</b></color> {{0}}";
        [SerializeField]
        private float damageMultiplier;

        private DamageablePartController controller;

        public void Init(Health health)
        {
            controller = new DamageablePartController(health, damageMultiplier);
        }

        public void Damage(float amount)
        {
            Debug.LogFormat(LOG_FORMAT, "Got " + amount + " damage");
            controller.Damage(amount);
        }
    }
}
