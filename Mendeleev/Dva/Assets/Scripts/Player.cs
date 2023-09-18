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

        private void OnCollisionEnter(Collision collision)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            if(collision.gameObject.GetComponent<SpecialParticle>() != null)
            {
                _gameControl.EventCall(collision.gameObject.GetComponent<SpecialParticle>().SpecialType);
                StartCoroutine(SetDestroySpecial(collision.gameObject));
            }
            StartCoroutine(SetDestroy(collision.gameObject));
        }

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
    }
}