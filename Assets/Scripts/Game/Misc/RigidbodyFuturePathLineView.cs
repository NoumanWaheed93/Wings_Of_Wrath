using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RigidbodyFuturePathLineView : MonoBehaviour
    {
        [SerializeField]
        private ArcLineRenderer arcLineRenderer;

        [SerializeField]
        private Rigidbody rigidbody;

        // Update is called once per frame
        void Update()
        {
            arcLineRenderer.linearVelocity = rigidbody.velocity.magnitude;
            arcLineRenderer.angularVelocity = rigidbody.angularVelocity.y;
        }
    }
}
