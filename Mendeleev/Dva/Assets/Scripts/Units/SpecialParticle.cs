using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class SpecialParticle : Particle
    {
        [SerializeField]  private SpecialParticleType _specialType;

        public SpecialParticleType SpecialType => _specialType;

        protected override void Awake()
        {
            base.Awake();
            _pointToGo = RandomPatrolPoint();
        }

        private void Update()
        {
            TaskPatrol();
        }

        //coordinate for particle to move
        protected override void TaskPatrol()
        {
            if (_specialType != SpecialParticleType.FastNeutron)
            {
                WanderTowardPatrolPoint(_gameFeatures.ParticleSpeed * 0.7f);
            }
            else
            {
                if (Vector3.Distance(transform.position, _pointToGo) < 0.05f)
                {
                    _gameManager.NeutronFastCounter.Remove(gameObject);
                    _gameManager.ReturnSpecialParticle(_specialType, gameObject);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, _pointToGo, _gameFeatures.ParticleSpeed * 3f);
                }
            }
        }

        //remove particle after hitting the atom (returned to the pool, not destroyed)
        protected override void RemoveEvent()
        {
            _gameManager.ReturnSpecialParticle(_specialType, gameObject);
        }
    }
}