using System.Collections;
using UnityEngine;

namespace Dva
{
    // Owns the black hole / particle-speed / field-resize / fast-neutron special-event coroutines.
    // Coroutines need a MonoBehaviour host to run on, so this takes GameControl for that purpose.
    public class FieldEventRunner
    {
        private readonly GameControl _gameControl;
        private readonly FeaturesManager _featuresManager;
        private readonly ParticleSpawner _particleSpawner;
        private readonly Player _player;
        private readonly Atom _atom;

        private bool _needToRemove;
        private bool _isBlackHoleActive;

        public bool IsBlackHoleActive => _isBlackHoleActive;

        public FieldEventRunner(GameControl gameControl, FeaturesManager featuresManager, ParticleSpawner particleSpawner, Player player, Atom atom)
        {
            _gameControl = gameControl;
            _featuresManager = featuresManager;
            _particleSpawner = particleSpawner;
            _player = player;
            _atom = atom;
        }

        //special particle eventcall
        public void Trigger(SpecialParticleType particle)
        {
            if (particle == SpecialParticleType.BlackHole)
            {
                _gameControl.StartCoroutine(BlackHoleEvent());
            }
            else if (particle == SpecialParticleType.TimeFast || particle == SpecialParticleType.TimeSlow)
            {
                _gameControl.StartCoroutine(SpeedChangeEvent(particle));
            }
            else if (particle == SpecialParticleType.FieldRise || particle == SpecialParticleType.FiledShrink)
            {
                _gameControl.StartCoroutine(FieldSizeChangeEvent(particle));
            }
            else if (particle == SpecialParticleType.FastNeutron)
            {
                FastNeutronEvent();
            }
        }

        //creatinf a zone around the atom where all particles draging into atom
        private IEnumerator BlackHoleEvent()
        {

            for (int i = _featuresManager.EventCountDown*10; i > 0;)
            {
                _isBlackHoleActive = true;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.transform.position, _featuresManager.FOVRange);

                if (colliders.Length > 0)
                {
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.TryGetComponent(out GeneralParticle particle))
                        {
                            particle._toPatrol = false;
                            particle._toBlackHole = true;
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
                i--;
            }

            _atom.EventAtomUpdate(true);
            _isBlackHoleActive = false;
        }

        //increase/decrease speed of particles for certain amount of time
        private IEnumerator SpeedChangeEvent(SpecialParticleType particle)
        {
            float startSpeed = _featuresManager.ParticleSpeed;
            float multiplier;
            if (particle == SpecialParticleType.TimeSlow)
            {
                multiplier = 0.7f;
            }
            else
            {
                multiplier = 10f;
            }
            for (int i = _featuresManager.EventCountDown; i >= 0;)
            {
                _featuresManager.ParticleSpeed = startSpeed * multiplier;
                yield return new WaitForSeconds(1f);
                i--;
            }
            _featuresManager.ParticleSpeed = _featuresManager.ParticleSpeed / multiplier;
        }

        //increase/decrease the size of the field for certain amount of time
        private IEnumerator FieldSizeChangeEvent(SpecialParticleType particle)
        {
            //need to destroy the old particles and respawn them, both now and again once the event ends
            float sizeMultiplier;
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.y;
            float bottom = _featuresManager.BottomBoarder.transform.position.y;
            Animator _fieldAnimator = _featuresManager.Field.GetComponent<Animator>();

            if (particle == SpecialParticleType.FiledShrink)
            {
                sizeMultiplier = 0.7f;
            }
            else
            {
                sizeMultiplier = 1.5f;
            }

            _needToRemove = true;

            for (int i = _featuresManager.EventCountDown * 3; i > 0;)
            {
                if (_needToRemove)
                {
                    if (particle == SpecialParticleType.FiledShrink)
                    {
                        _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x * sizeMultiplier, _player.gameObject.transform.position.y, _player.gameObject.transform.position.z * sizeMultiplier);
                        _fieldAnimator.SetBool("Smaller", true);
                    }
                    else
                    {
                        _fieldAnimator.SetBool("Bigger", true);
                    }
                    _featuresManager.ScaleBorders(left, right, top, bottom, sizeMultiplier);

                    RemoveAllFieldParticles();

                    _needToRemove = false;
                }

                yield return new WaitForSeconds(1f);
                i--;
            }


            _featuresManager.ScaleBorders(left, right, top, bottom, 1f / sizeMultiplier);

            if (particle == SpecialParticleType.FieldRise)
            {

                _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x / sizeMultiplier, _player.gameObject.transform.position.y, _player.gameObject.transform.position.z / sizeMultiplier);
                _fieldAnimator.SetBool("Bigger", false);
                _needToRemove = true;

                if (_needToRemove)
                {
                    RemoveAllFieldParticles();

                    _needToRemove = false;
                }
            }
            else
            {
                _fieldAnimator.SetBool("Smaller", false);
            }

        }

        private void RemoveAllFieldParticles()
        {
            _particleSpawner.RemoveForEvent(false, _particleSpawner.ParticlesCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.TimeFastCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.TimeSlowCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.BlackHolesCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.FieldBiggerCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.FieldSmallerCounter);
            _particleSpawner.RemoveForEvent(true, _particleSpawner.NeutronFastCounter);
        }

        //hit the atom by the fast neutron (just making the atom to decay)
        private void FastNeutronEvent()
        {
            _atom.EventAtomUpdate(false);
        }
    }
}
