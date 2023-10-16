using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class ElectronOrbit : MonoBehaviour
    {
        private GameObject _player;
        private FeaturesManager _features;
        [SerializeField] private Vector3 _orbit;

        void Awake()
        {
            _player = FindObjectOfType<Player>().gameObject;
            _features = FindObjectOfType<FeaturesManager>();
        }

        private void Update()
        {
            ElectronOrbitMove();
        }

        private void ElectronOrbitMove()
        {
            transform.RotateAround(_player.transform.position, _orbit, _features.ElectronOrbitSpeed * Time.deltaTime);
        }
    }
}