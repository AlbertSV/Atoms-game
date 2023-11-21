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
    _gameFeatures.TopBoarder.position.y, _gameFeatures.BottomBoarder.position.y);
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


        //get the coordinate for particle where to move
        protected override void TaskPatrol()
        {
            if (Vector3.Distance(transform.position, _pointToGo) < 0.01f)
            {
                transform.position = _pointToGo;
                _pointToGo = _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
                _gameFeatures.TopBoarder.position.y, _gameFeatures.BottomBoarder.position.y);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _pointToGo, _gameFeatures.ParticleSpeed * Time.deltaTime);
            }
        }

        //go to the player, if it has black hole event and the particle if radius of it
        private void BlackHoleGravity()
        {

            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _gameFeatures.ParticleSpeed * Time.deltaTime);
        }

        //remove particle after hitting the atom
        protected override void RemoveEvent()
        {
            Destroy(gameObject);
        }
    }
}