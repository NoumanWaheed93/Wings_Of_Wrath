using UnityEngine;

namespace HealthSystem
{
    public class CollisionDamager : MonoBehaviour
    {
        private const string LOG_FORMAT = "<color=#00FF62><b>[CollisionDamager]</b></color> {{0}}";
        [SerializeField]
        private float damageAmount;

        [SerializeField]
        private bool isCollisionDamageEnabled;
        [SerializeField]
        private bool isTriggerDamageEnabled;

        private HitDamager hitDamager;

        private void Awake()
        {
            hitDamager = new HitDamager(damageAmount);
            Health health = new Health(10, 10);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.LogFormat(LOG_FORMAT, "OnCollisionEnter()");
            if (isCollisionDamageEnabled == false)
                return;

            IDamageable damageable;
            if(collision.collider.TryGetComponent(out damageable))
            {
                hitDamager.Hit(damageable);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat(LOG_FORMAT, "OnTriggerEnter()");
            if (isTriggerDamageEnabled == false) 
                return;

            if (other.isTrigger)
                return;


            IDamageable damageable;
            if(other.TryGetComponent(out damageable))
            {
                hitDamager.Hit(damageable);
            }
        }
    }
}
