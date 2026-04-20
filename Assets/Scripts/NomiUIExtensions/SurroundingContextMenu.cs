using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NomiUIExtensions
{
    public class SurroundingContextMenu : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private Transform maxIndicator;

        [SerializeField]
        private CanvasGroup canvasGroup;

        private Camera cam;

        private MeshRenderer meshRenderer; //The mesh that will be surrounded be the menu elements.

        private void Awake()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            if(meshRenderer == null)
            {
                return;
            }

            Init(meshRenderer.bounds);
        }

        public void SetMeshRenderer(MeshRenderer meshRenderer)
        {
            this.meshRenderer = meshRenderer;
        }

        public void ShowUp()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Init(Bounds bounds)
        {
            transform.position = GetScreenPosition(bounds.center);
            Vector3 extremePosition = GetScreenPosition(bounds.max);
            maxIndicator.position = extremePosition;
            float maxDistance = Vector3.Distance(transform.position, extremePosition);
            rectTransform.sizeDelta = new Vector3(maxDistance * 2, maxDistance * 2, 1);
        }

        private Vector3 GetScreenPosition(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(cam, position);
        }
    }
}
