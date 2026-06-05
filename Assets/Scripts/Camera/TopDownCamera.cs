using UnityEngine;
using Common;

namespace CameraController
{
    public class TopDownCamera : MonoBehaviour, ITargetTransformReceiver
    {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private float smoothTime;
        [SerializeField]
        private Transform target;
        public Transform Target
        {
            get => target;
            set => target = value;
        }


        private Vector3 velocity;

        private void LateUpdate()
        {
            SetPosition();
            SetRotation();
        }

        private void SetPosition()
        {
            Vector3 newPosition = target.position + (target.forward * offset.z) + new Vector3(0, offset.y, 0);
            newPosition = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
            transform.position = newPosition;
        }

        private void SetRotation()
        {
            Quaternion newRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
            newRotation = Quaternion.Slerp(transform.rotation, newRotation, 30f * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }
}