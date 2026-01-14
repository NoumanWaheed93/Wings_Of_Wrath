using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace ScreenInputControls
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onHold;

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

        private void Update()
        {
            if(isHeldDown)
            {
                onHold?.Invoke();
            }
        }
    }
}
 