using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace ScreenInputControls
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const string LOG_FORMAT = "<color=#D9CE00><b>[HoldableButton]</b></color> {0}";
        private string gameObjectName;
        private string GameObjectName
        {
            get
            {
                if (string.IsNullOrEmpty(gameObjectName))
                {
                    gameObjectName = $"<color=#F9CE00> ->{gameObject.name}<- </color>";
                }
                return gameObjectName;
            }
        }
        public UnityEvent onHold;

        public event Action OnHoldStart;
        public event Action OnHoldEnd;

        public bool isHeldDown { get; private set; }

        public virtual void OnPointerDown(PointerEventData eventData) 
        {
            Debug.LogFormat(GameObjectName + LOG_FORMAT, "OnPointerDown()");
            isHeldDown = true;
            OnHoldStart?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData) 
        {
            Debug.LogFormat(GameObjectName + LOG_FORMAT, "OnPointerUp()");
            isHeldDown = false;
            OnHoldEnd?.Invoke();
        }

        protected virtual void Update()
        {
         //   Debug.LogFormat(GameObjectName + LOG_FORMAT, $"Update(): isHeldDown = {isHeldDown}");
            if (isHeldDown)
            {
                onHold?.Invoke();
            }
        }
    }
}
 