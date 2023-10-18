using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dva
{
    public class CameraMove : MonoBehaviour
    {
        private Vector2 _touchStart;
        private Vector2 _moveVector;
        private bool _hold;

        private void FixedUpdate()
        {
            DragCamera();
        }

        private void DragCamera()
        {
            if (Input.touchCount == 0)
            {
                if (_hold)
                {
                    _moveVector = new Vector2(_touchStart.x - Camera.main.ScreenToViewportPoint(Input.touches[0].position).x, _touchStart.y - Camera.main.ScreenToViewportPoint(Input.touches[0].position).y);
                    _hold = false;
                }
            }
            else
            {
                if (Input.touchCount == 1 && !_hold)            
                {
                    _hold = true;
                    _touchStart = Camera.main.ScreenToViewportPoint(Input.touches[0].position);
                }
            }


            transform.Translate(_moveVector * Time.deltaTime * 20);
        }
    }
}
