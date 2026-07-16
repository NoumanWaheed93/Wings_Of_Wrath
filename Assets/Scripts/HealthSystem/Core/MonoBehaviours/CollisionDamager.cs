using UnityEngine;
using System;

namespace HealthSystem
{
    public class CollisionDamager : MonoBehaviour
    {
        public event Action OnCollided;

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

            OnCollided?.Invoke(); //We have to notify the collision even if it does not damage. Because, collision affects physics

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
                OnCollided?.Invoke(); //We have to notify the collision only when the damage is applied because, trigger do not affect physics.
            }
        }
    }
}
