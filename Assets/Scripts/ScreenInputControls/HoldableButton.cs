using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace ScreenInputControls
{
    public abstract class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnHoldStart;
        public event Action OnHoldEnd;

        public bool isHeldDown { get; private set; }

        public virtual void OnPointerDown(PointerEventData eventData) 
        {
            isHeldDown = true;
            OnHoldStart?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData) 
        {
            isHeldDown = false;
            OnHoldEnd?.Invoke();
        }
    }
}
