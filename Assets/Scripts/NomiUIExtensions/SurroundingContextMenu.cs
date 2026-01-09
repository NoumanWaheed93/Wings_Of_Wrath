using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NomiUIExtensions
{
    public class SurroundingContextMenu : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private Transform maxIndicator;

        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            Init(_meshRenderer.bounds);
        }

        public void ShowUp()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Init(Bounds bounds)
        {
            transform.position = GetScreenPosition(bounds.center);
            Vector3 extremePosition = GetScreenPosition(bounds.max);
            maxIndicator.position = extremePosition;
            float maxDistance = Vector3.Distance(transform.position, extremePosition);
            _rectTransform.sizeDelta = new Vector3(maxDistance * 2, maxDistance * 2, 1);
        }

        private Vector3 GetScreenPosition(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(cam, position);
        }
    }
}
