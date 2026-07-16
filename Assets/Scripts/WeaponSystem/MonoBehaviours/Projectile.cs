using UnityEngine;

namespace WeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        protected Rigidbody m_rigidbody;
        protected Transform m_transform;

        public Vector3 position { get { return m_transform.position; } set { m_transform.position = value; } }
        public Quaternion rotation { get { return m_transform.rotation; } set { m_transform.rotation = value; } }
        public Vector3 forward { get => m_transform.forward; }
        public Vector3 right { get => m_transform.right; }

        protected virtual void Awake()
        {
            this.m_transform = base.transform;
            m_rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void FixedUpdate()
        {
            m_rigidbody.linearVelocity = m_transform.forward * speed;
        }
    }
}
