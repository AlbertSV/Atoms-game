using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dva
{
    public class MoveController : MonoBehaviour
    {
        private PlayerControl _plaeyrController;
        private Rigidbody _playerParticleRigdBody;
        private Vector2 _moveInput;

        [SerializeField]
        private float PLAYERSPEED = 0.1f;

        private void Awake()
        {
            _plaeyrController = new PlayerControl();
        }


        void Start()
        {
            _playerParticleRigdBody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _plaeyrController.KeyBoard.Enable();
            _plaeyrController.Sensor.Enable();
        }

        private void FixedUpdate()
        {
            _moveInput = _plaeyrController.KeyBoard.Move.ReadValue<Vector2>();
            //_moveInput = _plaeyrController.Sensor.Move.ReadValue<Vector2>();
            //Debug.Log(_moveInput);
            Move(_playerParticleRigdBody);
        }

        private void OnDisable()
        {
            _plaeyrController.KeyBoard.Disable();
            _plaeyrController.Sensor.Disable();
        }

        private void Move(Rigidbody player)
        {
            player.AddForce(_moveInput.x * PLAYERSPEED * transform.right);
            player.AddForce(_moveInput.y * PLAYERSPEED * transform.forward);
        }
    }
}