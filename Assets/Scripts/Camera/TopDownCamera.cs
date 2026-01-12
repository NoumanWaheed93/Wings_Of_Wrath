using UnityEngine;

namespace CameraController
{
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private Transform target;

        private void LateUpdate()
        {
            SetPosition();
            SetRotation();
        }

        private void SetPosition()
        {
            Vector3 newPosition = target.position + (target.forward * offset.z) + new Vector3(0, offset.y, 0);
            newPosition = Vector3.Lerp(transform.position, newPosition, 10f * Time.deltaTime);
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