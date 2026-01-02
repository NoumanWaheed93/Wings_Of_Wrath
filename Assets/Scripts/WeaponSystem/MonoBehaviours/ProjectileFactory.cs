using UnityEngine;

namespace WeaponSystem
{
    public class ProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        [SerializeField]
        private GameObject simpleProjectilePrefab;
        [SerializeField]
        private GameObject homingProjectilePrefab;

        public IGuidedProjectile GetHomingProjectile()
        {
            return Instantiate(homingProjectilePrefab).GetComponent<IGuidedProjectile>();
        }

        public Transform GetProjectile()
        {
            return Instantiate(simpleProjectilePrefab).transform;
        }
    }
}
