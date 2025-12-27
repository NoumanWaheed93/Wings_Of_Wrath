using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealthSystem
{
    public class HealthMonobehaviour : MonoBehaviour
    {
        [SerializeField]
        private DamageablePart[] damageableParts;

        [SerializeField]
        private float maxHealth;

        private Health health;

        private void Awake()
        {
            health = new Health(maxHealth, maxHealth);
            foreach (DamageablePart part in damageableParts)
            {
                part.Init(health);
            }
        }
    }
}
