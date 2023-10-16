using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
    }
}