using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

namespace Dva
{
    public class SpecialParticle : Particle
    {
        private float _specialParticleSpeed;
        [SerializeField]  private SpecialParticleType _specialType;

        private Atom _atom;

        public SpecialParticleType SpecialType => _specialType;

        protected override void Awake()
        {
            base.Awake();
            _pointToGo = _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
    _gameFeatures.TopBoarder.position.z, _gameFeatures.BottomBoarder.position.z);

        }

        private void Start()
        {
            _atom = FindObjectOfType<Atom>();
        }
    

        private void Update()
        {
            TaskPatrol();
        }


        protected override void TaskPatrol()
        {
            if (_specialType != SpecialParticleType.FastNeutron)
            {
                if (Vector3.Distance(transform.position, _pointToGo) < 0.01f)
                {
                    transform.position = _pointToGo;
                    _pointToGo = _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
                    _gameFeatures.TopBoarder.position.z, _gameFeatures.BottomBoarder.position.z);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, _pointToGo, _gameFeatures.ParticleSpeed * 0.7f);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, _pointToGo) < 0.05f)
                {
                    _gameManager._neutronFastCounter.Remove(gameObject);
                    Destroy(gameObject);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, _pointToGo, _gameFeatures.ParticleSpeed * 3f);
                }
            }
        }

        protected override void RemoveEvent()
        {
            Destroy(gameObject);
        }
    }
}