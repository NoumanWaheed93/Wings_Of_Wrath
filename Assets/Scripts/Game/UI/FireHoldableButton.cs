using ScreenInputControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    public class FireHoldableButton : HoldableButton//, IPointerMoveHandler
    {
        private const string LOG_FORMAT = "<color=#FF7800><b>[FireHoldableButton]</b></color> {0}";
        [Inject]
        private ThumbDriftInput _thumbDriftInput;

        [SerializeField]
        private CanvasGroup buttonCanvasGroup;

        public override void OnPointerDown(PointerEventData eventData)
        {
            Debug.LogFormat(LOG_FORMAT, "OnPointerDown");
            base.OnPointerDown(eventData);
            _thumbDriftInput.OnPointerDown(eventData);
            buttonCanvasGroup.alpha = 0.1f;
            buttonCanvasGroup.blocksRaycasts = false;
            buttonCanvasGroup.ignoreParentGroups = true;
        }

        //public void OnPointerMove(PointerEventData eventData)
        //{
        //    if (!isHeldDown)
        //        return;

        //    transform.position = eventData.position;
        //}

        public override void OnPointerUp(PointerEventData eventData)
        {
            Debug.LogFormat(LOG_FORMAT, "OnPointerUp");
            base.OnPointerUp(eventData);
            _thumbDriftInput.OnPointerUp(eventData);
            buttonCanvasGroup.alpha = 1;
            buttonCanvasGroup.blocksRaycasts = true;
            buttonCanvasGroup.ignoreParentGroups = false;
        }
    }
}
