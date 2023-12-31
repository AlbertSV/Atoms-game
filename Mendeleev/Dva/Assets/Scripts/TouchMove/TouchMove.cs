using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Dva
{
    public class TouchMove : MonoBehaviour
    {
        [SerializeField] private Vector2 _joystickSize = new Vector2(100, 100);
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private Rigidbody2D _player;
        [SerializeField]
        private float PLAYERSPEED = 0.1f;
        [SerializeField] private float _maxSpeed;

        private Finger _movementFinger;
        private Vector2 _movementAmount;

        private void Update()
        {
            MoveUpdate();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            ETouch.Touch.onFingerDown += HandleFingerDown;
            ETouch.Touch.onFingerUp += HandleLoseFinger;
            ETouch.Touch.onFingerMove += HandleFingerMove;
        }

        private void OnDisable()
        {
            ETouch.Touch.onFingerDown -= HandleFingerDown;
            ETouch.Touch.onFingerUp -= HandleLoseFinger;
            ETouch.Touch.onFingerMove -= HandleFingerMove;
            EnhancedTouchSupport.Disable();
        }

        //catch the direction and max movement speed
        private void HandleFingerMove(Finger MovedFinger)
        {
            if(MovedFinger == _movementFinger)
            {
                Vector2 KnobPosition;
                float maxMovement = _joystickSize.x / 2f;
                ETouch.Touch currentTouch = MovedFinger.currentTouch;

                if(Vector2.Distance(currentTouch.screenPosition, _joystick._rectTransform.anchoredPosition) > maxMovement)
                {
                    KnobPosition = (currentTouch.screenPosition - _joystick._rectTransform.anchoredPosition).normalized * maxMovement;
                }
                else
                {
                    KnobPosition = (currentTouch.screenPosition - _joystick._rectTransform.anchoredPosition);
                }

                _joystick._knob.anchoredPosition = KnobPosition;
                _movementAmount = KnobPosition / maxMovement;
            }
        }


        //the action if the finger is not touching the screen
        private void HandleLoseFinger(Finger LostFinger)
        {
            if(LostFinger == _movementFinger)
            {
                _movementFinger = null;
                _joystick._knob.anchoredPosition = Vector2.zero;
                _joystick.gameObject.SetActive(false);
                _movementAmount = Vector2.zero; 
            }
        }

        //spot the place where the finger touched the screen
        private void HandleFingerDown(Finger TouchedFinger)
        {
            if(Time.timeScale == 1)
            {
                if (_movementFinger == null && TouchedFinger.screenPosition.x <= Screen.width / 2f)
                {
                    _movementFinger = TouchedFinger;
                    _movementAmount = Vector2.zero;
                    _joystick.gameObject.SetActive(true);
                    _joystick._rectTransform.sizeDelta = _joystickSize;
                    _joystick._rectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
                }
            }
        }

        //making start position set into left part of the screen
        private Vector2 ClampStartPosition(Vector2 StartPosition)
        {
             if(StartPosition.x <_joystickSize.x /2)
             {
                StartPosition.x = _joystickSize.x / 2;
             }

             if(StartPosition.y < _joystickSize.y /2)
             {
                StartPosition.y = _joystickSize.y / 2;
             }
             else if (StartPosition.y > Screen.height - _joystickSize.y / 2)
             {
                StartPosition.y = Screen.height - _joystickSize.y / 2;
             }

            return StartPosition;
        }

        //making player move
        private void  MoveUpdate()
        {
            Vector2 scaledMovement = PLAYERSPEED * Time.deltaTime * new Vector2(_movementAmount.x, _movementAmount.y);
            _player.AddForce(scaledMovement);

            if(_player.velocity.magnitude > _maxSpeed)
            {
                _player.velocity = Vector2.ClampMagnitude(_player.velocity, _maxSpeed);
            }
        }
    }
}