using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class GeneralParticle : Particle
    {
        [SerializeField] private GeneralParticleType _generalType;

        public GeneralParticleType GeneralType => _generalType;
        public bool _toPatrol = true;
        public bool _toBlackHole = false;
        private Player _player;

        protected override void Awake()
        {
            base.Awake();
            _pointToGo = _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
    _gameFeatures.TopBoarder.position.z, _gameFeatures.BottomBoarder.position.z);
            _player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            if (_toPatrol)
            {
                TaskPatrol();
            }
            else if (_toBlackHole)
            {
                BlackHoleGravity();
            }
        }

        protected override void TaskPatrol()
        {
            if (Vector3.Distance(transform.position, _pointToGo) < 0.01f)
            {
                transform.position = _pointToGo;
                _pointToGo = _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
                _gameFeatures.TopBoarder.position.z, _gameFeatures.BottomBoarder.position.z);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _pointToGo, _gameFeatures.ParticleSpeed);
            }
        }

        private void BlackHoleGravity()
        {

            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _gameFeatures.ParticleSpeed);
        }

        protected override void RemoveEvent()
        {
            Destroy(gameObject);
        }
    }
}