using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class Player : Atom
    {

        private GameControl _gameControl;

        private void Awake()
        {
            _gameControl = FindObjectOfType<GameControl>();
        }

        //actions when the particles hits the atom
        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject hit = collision.gameObject;

            if (hit.TryGetComponent(out CircleCollider2D hitCollider))
            {
                hitCollider.enabled = false;
            }

            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

            bool isSpecial = hit.TryGetComponent(out SpecialParticle specialParticle);
            bool isGeneral = hit.TryGetComponent(out GeneralParticle _);

            if (isSpecial || isGeneral)
            {
                _audioController.AudioPlay(true, _featuresManager.AbsorbAudio);
            }

            if (isSpecial)
            {
                _gameControl.EventCall(specialParticle.SpecialType);
                StartCoroutine(SetDestroySpecial(hit));
            }
            else
            {
                StartCoroutine(SetDestroy(hit));
            }

        }

        //destroy hitted particle
        public IEnumerator SetDestroy(GameObject objectTriggered)
        {
            if (objectTriggered.TryGetComponent(out GeneralParticle particle))
            {
                _gameControl.ParticleCounter.Remove(objectTriggered);
                IDUpdate(particle);
                Animator animator = objectTriggered.GetComponent<Animator>();
                animator.SetBool("ToRemove", true);
                yield return new WaitForSeconds(0f);
            }
        }

        //destroy special particle
        public IEnumerator SetDestroySpecial(GameObject objectTriggered)
        {
            if (objectTriggered.TryGetComponent(out SpecialParticle special))
            {
                switch (special.SpecialType)
                {
                    case SpecialParticleType.BlackHole:
                        _gameControl.BlackHolesCounter.Remove(objectTriggered);
                        break;
                    case SpecialParticleType.TimeFast:
                        _gameControl.TimeFastCounter.Remove(objectTriggered);
                        break;
                    case SpecialParticleType.TimeSlow:
                        _gameControl.TimeSlowCounter.Remove(objectTriggered);
                        break;
                    case SpecialParticleType.FiledShrink:
                        _gameControl.FieldSmallerCounter.Remove(objectTriggered);
                        break;
                    case SpecialParticleType.FieldRise:
                        _gameControl.FieldBiggerCounter.Remove(objectTriggered);
                        break;
                    case SpecialParticleType.Lives:
                        _gameControl.LivesCounter.Remove(objectTriggered);
                        break;
                    default:
                        _gameControl.NeutronFastCounter.Remove(objectTriggered);
                        break;
                }

                Animator animator = objectTriggered.GetComponent<Animator>();
                animator.SetBool("ToRemove", true);
                yield return new WaitForSeconds(0f);
            }
        }

        private int IDUpdate(GeneralParticle particle)
        {
            if (particle.GeneralType == GeneralParticleType.Electron)
            {
                _eAmount += 1;
            }
            else if(particle.GeneralType == GeneralParticleType.Neutron)
            {
                _nAmount += 1;
            }
            else
            {
                _pAmount += 1;
            }

            _atomID = AtomIDUpdate();
            Debug.Log(_atomID);
            CompositionUpdate();

            if(_gameControl.IsBlackHoleActive)
            {
                return _atomID;
            }
            else
            {
                AtomCharge();
                return _atomID;
            }

        }
    }
}