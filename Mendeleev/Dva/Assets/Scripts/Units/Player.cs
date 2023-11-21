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

            if(_eAmount <6)
            {
               // ElectronCreate();
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
                    _gameControl._blackHolesCounter.Remove(objectTriggered);
                }
                else if(objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.TimeFast)
                {
                    _gameControl._timeFastCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.TimeSlow)
                {
                    _gameControl._timeSlowCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.FiledShrink)
                {
                    _gameControl._fieldSmallerCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.FieldRise)
                {
                    _gameControl._fieldBiggerCounter.Remove(objectTriggered);
                }
                else if (objectTriggered.GetComponent<SpecialParticle>().SpecialType == SpecialParticleType.Lives)
                {
                    _gameControl._livesCounter.Remove(objectTriggered);
                }
                else
                {
                    _gameControl._neutronFastCounter.Remove(objectTriggered);
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

        //create electron on atoms orbit
        private void ElectronCreate()
        {
            if (_eAmount == 2)
            {
                transform.GetChild(3).gameObject.SetActive(true);
                transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (_eAmount == 3)
            {
                transform.GetChild(4).gameObject.SetActive(true);
                transform.GetChild(5).gameObject.SetActive(false);
            }
            else if (_eAmount == 4)
            {
                transform.GetChild(5).gameObject.SetActive(true);
                transform.GetChild(6).gameObject.SetActive(false);
            }
            else if (_eAmount == 5)
            {
                transform.GetChild(6).gameObject.SetActive(true);
                transform.GetChild(7).gameObject.SetActive(false);
            }
            else if (_eAmount == 6)
            {
                transform.GetChild(7).gameObject.SetActive(true);
            }
            else if (_eAmount < 2)
            {
                transform.GetChild(3).gameObject.SetActive(false);
            }
        }

    }
}