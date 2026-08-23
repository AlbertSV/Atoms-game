using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    // Owns the on-screen life icons: creates the starting set, and adds one when a "lives"
    // pickup is collected (also resetting that pickup's spawn cooldown via ParticleSpawner).
    public class LivesController
    {
        private readonly ParticleSpawner _particleSpawner;
        private readonly GameObject _livesCanvas;
        private readonly Transform _livesHolder;
        private readonly float _liveStepCanvas = 0.1f;

        public List<GameObject> LivesList { get; } = new List<GameObject>();

        public LivesController(ParticleSpawner particleSpawner, GameObject livesCanvas, Transform livesHolder, int startingLives)
        {
            _particleSpawner = particleSpawner;
            _livesCanvas = livesCanvas;
            _livesHolder = livesHolder;

            for (int i = 0; i < startingLives; i++)
            {
                GameObject life = Object.Instantiate(_livesCanvas, new Vector3(_livesHolder.position.x + i * _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                    , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
            }
        }

        //add a life icon after a "lives" special particle is collected
        public void LivesEvent()
        {
            if (LivesList.Count < 5)
            {
                Transform lastLifePosition = LivesList[LivesList.Count - 1].gameObject.transform;
                GameObject life = Object.Instantiate(_livesCanvas, new Vector3(lastLifePosition.position.x + _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                        , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
                _particleSpawner.ResetLivesTimer();
            }
        }
    }
}
