using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DevPenguin.Utilities
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onButtonDownEvent;
        public UnityEvent onButtonUpEvent;

        private bool _isPointerDown;

        // Update is called once per frame
        void Update()
        {
            if (_isPointerDown)
            {
                onButtonDownEvent.Invoke();
            }
            else
            {
                onButtonUpEvent.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
        }
    }
}
