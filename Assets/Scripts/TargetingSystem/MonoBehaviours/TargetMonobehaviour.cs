using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetingSystem
{
    public class TargetMonobehaviour : MonoBehaviour, ITargetable
    {
        [SerializeField]
        private Team team;

        public Team Team => team;

        public Transform Transform => mTransform;

        private Transform mTransform;

        public Vector3 position { get => mTransform.position; set => mTransform.position = value; }
        public Quaternion rotation { get => mTransform.rotation; set => mTransform.rotation = value; }

        public Vector3 forward => mTransform.forward;

        public Vector3 right => mTransform.right;

        private void Awake()
        {
            mTransform = transform;
        }
    }
}
