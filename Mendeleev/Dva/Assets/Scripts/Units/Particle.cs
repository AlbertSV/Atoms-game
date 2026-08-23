using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class Particle : MonoBehaviour
    {

        protected GameControl _gameManager;
        protected Vector3 _pointToGo;
        protected FeaturesManager _gameFeatures;



        protected virtual void Awake()
        {
            _gameManager = FindObjectOfType<GameControl>();
            _gameFeatures = FindObjectOfType<FeaturesManager>();

        }

        protected virtual void TaskPatrol()
        {

        }

        protected virtual void RemoveEvent()
        {

        }

        //pick a random point within the field for a particle to wander toward
        protected Vector3 RandomPatrolPoint()
        {
            return _gameManager.GetRandomPosition(_gameFeatures.LeftBoarder.position.x, _gameFeatures.RightBoarder.position.x,
                _gameFeatures.TopBoarder.position.y, _gameFeatures.BottomBoarder.position.y);
        }

        //move toward _pointToGo at the given speed, snapping to it and picking a new one on arrival
        protected void WanderTowardPatrolPoint(float speed)
        {
            if (Vector3.Distance(transform.position, _pointToGo) < 0.01f)
            {
                transform.position = _pointToGo;
                _pointToGo = RandomPatrolPoint();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _pointToGo, speed);
            }
        }
    }
}