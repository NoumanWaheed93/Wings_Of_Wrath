using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TargetingSystem
{
    public class TargetIconView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite sprite_OffScreen;

        [SerializeField]
        private Vector2 size_OffScreen = new Vector2(8f, 16f);

        [SerializeField, Tooltip("Gap between the off-screen icon and the screen edge, in canvas units.")]
        private float edgePadding = 4f;

        public event Action<ITargetable> OnTargetClicked;
        private ITargetable target;
        private Transform radarTransform;

        private RectTransform rectTransform;
        private Canvas canvas;
        private Sprite sprite_OnScreen;
        private Vector2 size_OnScreen;
        private Vector2 pivot_OnScreen;
        private bool isOffScreen;

        public void Initialize(ITargetable target, Transform radarTransform)
        {
            this.target = target;
            this.radarTransform = radarTransform;

            rectTransform = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>().rootCanvas;
            sprite_OnScreen = image.sprite;
            size_OnScreen = rectTransform.sizeDelta;
            pivot_OnScreen = rectTransform.pivot;
        }

        private void LateUpdate()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            Camera cam = Camera.main;
            Vector3 screenPoint = cam.WorldToScreenPoint(target.Transform.position);

            Rect screen = new Rect(0, 0, Screen.width, Screen.height);
            bool offScreen = screenPoint.z < 0 || !screen.Contains(screenPoint);
            SetOffScreen(offScreen);

            if (!offScreen)
            {
                transform.position = (Vector2)screenPoint;
                return;
            }

            Vector2 center = screen.center;
            Vector2 direction = (Vector2)screenPoint - center;

            // Behind the camera the projection is mirrored.
            if (screenPoint.z < 0)
                direction = -direction;

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector2.down;

            float scale = canvas.scaleFactor;
            float margin = (Mathf.Max(size_OffScreen.x, size_OffScreen.y) * 0.5f + edgePadding) * scale;
            Vector2 halfExtents = new Vector2(
                Mathf.Max(0f, screen.width * 0.5f - margin),
                Mathf.Max(0f, screen.height * 0.5f - margin));

            // Slide along the ray from the screen center until it hits the inset screen rectangle.
            float tx = direction.x != 0 ? halfExtents.x / Mathf.Abs(direction.x) : float.PositiveInfinity;
            float ty = direction.y != 0 ? halfExtents.y / Mathf.Abs(direction.y) : float.PositiveInfinity;
            transform.position = center + direction * Mathf.Min(tx, ty);
        }

        private void SetOffScreen(bool offScreen)
        {
            if (offScreen == isOffScreen)
                return;

            isOffScreen = offScreen;
            image.sprite = offScreen && sprite_OffScreen != null ? sprite_OffScreen : sprite_OnScreen;
            rectTransform.sizeDelta = offScreen ? size_OffScreen : size_OnScreen;
            rectTransform.pivot = offScreen ? new Vector2(0.5f, 0.5f) : pivot_OnScreen;
        }

        private void UpdateRotation()
        {
            Vector3 relativeDirection = radarTransform.InverseTransformDirection(target.Transform.forward);
            float angle = Mathf.Atan2(relativeDirection.x, relativeDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }

        public void Highlight() 
        {
            image.color = Color.blue;
        }

        public void UnHighlight()
        {
            image.color = Color.red;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("On Click");

            OnTargetClicked?.Invoke(target);
        }
    }
}
