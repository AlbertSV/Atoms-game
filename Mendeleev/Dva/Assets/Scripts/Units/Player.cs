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
            if(collision.gameObject.GetComponent<CircleCollider2D>() != false)
            {
                collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            }

            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);


            if (collision.gameObject.GetComponent<SpecialParticle>() != null || collision.gameObject.GetComponent<GeneralParticle>() != null)
            {
                _audioController.AudioPlay(true, _featuresManager.AbsorbAudio);
            }

            if(collision.gameObject.GetComponent<SpecialParticle>() != null)
            {
                _gameControl.EventCall(collision.gameObject.GetComponent<SpecialParticle>().SpecialType);
                StartCoroutine(SetDestroySpecial(collision.gameObject));
            }
            else
            {
                StartCoroutine(SetDestroy(collision.gameObject));
            }

        }

        //destroy hitted particle
        public IEnumerator SetDestroy(GameObject objectTriggered)
        {
            if (objectTriggered.GetComponent<GeneralParticle>() != null)
            {
                _gameControl.ParticleCounter.Remove(objectTriggered);
                IDUpdate(objectTriggered);
                Animator animator = objectTriggered.GetComponent<Animator>();
                animator.SetBool("ToRemove", true);
                yield return new WaitForSeconds(0f);
            }
        }

        //destroy special particle
        public IEnumerator SetDestroySpecial(GameObject objectTriggered)
        {
            if (objectTriggered.GetComponent<SpecialParticle>() != null)
            {
                if(objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.BlackHole)
                {
                    _gameControl.BlackHolesCounter.Remove(objectTriggered);
                }
                else if(objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.TimeFast)
                {
                    _gameControl.TimeFastCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.TimeSlow)
                {
                    _gameControl.TimeSlowCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.FiledShrink)
                {
                    _gameControl.FieldSmallerCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.FieldRise)
                {
                    _gameControl.FieldBiggerCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.Lives)
                {
                    _gameControl.LivesCounter.Remove(objectTriggered);
                }
                else
                {
                    _gameControl.NeutronFastCounter.Remove(objectTriggered);
                }

                Animator animator = objectTriggered.GetComponent<Animator>();
                animator.SetBool("ToRemove", true);
                yield return new WaitForSeconds(0f);
            }
        }

        private int IDUpdate(GameObject particle)
        {
            if (particle.GetComponent<GeneralParticle>().GeneralType == GeneralParticleType.Electron)
            {
                _eAmount += 1;
            }
            else if(particle.GetComponent<GeneralParticle>().GeneralType == GeneralParticleType.Neutron)
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