using UnityEngine;

namespace HealthSystem
{
    public class CollisionDamager : MonoBehaviour
    {
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
            Debug.Log("OnCollisionEnter()");
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
            Debug.Log("OnTriggerEnter()");
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
