using UnityEngine;
using WeaponSystem;

namespace Game
{
    public class GuidedProjectileFacade : ProjectileFacade, IGuidedProjectile
    {
        private GuidedProjectile guidedProjectile;

        public Transform Target { get => guidedProjectile.Target; set => guidedProjectile.Target = value; }

        protected override void Awake()
        {
            base.Awake();
            guidedProjectile = GetComponent<GuidedProjectile>();
        }
    }
}
