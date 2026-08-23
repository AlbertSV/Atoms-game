using System.Collections;
using TMPro;
using UnityEngine;

namespace Dva
{
    // Owns the atom's balance-decay coroutine state machine: checks the electron/proton/neutron
    // balance, counts down before a decay, and applies the decay/explosion outcome.
    // Coroutines need a MonoBehaviour host to run on, so this takes Atom for that purpose.
    public class AtomDecayController
    {
        private readonly Atom _atom;
        private readonly GameControl _gameControl;
        private readonly FeaturesManager _featuresManager;
        private readonly AudioGameController _audioController;
        private readonly TMP_Text _atomDecayText;
        private readonly ParticleSystem _particleSystem;
        private readonly Animator _animator;
        private readonly int _decayCount;

        private bool _toDecay;
        private bool _inDecay;

        public AtomDecayController(Atom atom, GameControl gameControl, FeaturesManager featuresManager, AudioGameController audioController)
        {
            _atom = atom;
            _gameControl = gameControl;
            _featuresManager = featuresManager;
            _audioController = audioController;
            _atomDecayText = featuresManager.AtomDecay;
            _particleSystem = featuresManager.ParticleSystem;
            _animator = _particleSystem.GetComponentInParent<Animator>();
            _decayCount = featuresManager.DecayCountDown;
        }

        //check the charge of the atom, if it's not eaqual atom should decay
        public void CheckBalance()
        {
            if (_atom.EAmount == _atom.PAmount)
            {
                if (_atom.NAmount > _atom.EAmount * 2)
                {
                    _atom.StartCoroutine(AtomNeutronDecay());
                }
                else if (_atom.NAmount <= _atom.EAmount - 2)
                {
                    _atom.StartCoroutine(AtomNeutronDecay());
                }

                _atom.AtomUpgrade(_atom.AtomID);
                _atom.CompositionUpdate();
                _atom.AddScore(10);
            }
            else
            {
                if (_atom.NAmount == 1 && _atom.EAmount == 1 && _atom.PAmount == 0 || _atom.NAmount == 1 && _atom.EAmount == 0 && _atom.PAmount == 1)
                {
                    _atom.CompositionUpdate();
                    _atom.AddScore(10);
                }
                else
                {
                    _atom.StartCoroutine(ParticleStateAmountDecrease());
                }
            }
        }

        //atom decay process
        private IEnumerator ParticleStateAmountDecrease()
        {
            if (!_inDecay)
            {
                _toDecay = true;
                _inDecay = true;

                //counter before atom decay, the player has chance to save the atom
                for (int i = _decayCount; i >= 0;)
                {
                    _atomDecayText.text = "Decay in " + i;
                    _animator.SetBool("ToTrim", true);
                    _audioController.AudioPlay(true, _featuresManager.TicAudio);
                    yield return new WaitForSeconds(1f);
                    i--;

                    //in case the charge back to normal
                    if (_atom.EAmount == _atom.PAmount && _atom.NAmount >= _atom.EAmount - 2)
                    {
                        _audioController.AudioPlay(false, _featuresManager.TicAudio);
                        _toDecay = false;
                        _animator.SetBool("ToTrim", false);
                        _atomDecayText.text = "";
                        _atom.CompositionUpdate();
                        _inDecay = false;
                        break;
                    }
                }

                _atomDecayText.text = "";
                //if player coudn't save the atom
                if (_toDecay)
                {
                    _audioController.AudioPlay(false, _featuresManager.TicAudio);
                    _audioController.AudioPlay(true, _featuresManager.ExplodeAudio);
                    _animator.SetBool("ToTrim", false);
                    _particleSystem.Play();
                    _atom.AtomID = AtomDecay();
                    _atom.AtomUpgrade(_atom.AtomID);
                    _atom.CompositionUpdate();
                    _atom.ScaleScore(2f);
                    DestroyLife();
                    _inDecay = false;
                    _toDecay = false;
                }
            }
        }

        //set the amount of particles after decay
        private int AtomDecay()
        {
            {
                if (_atom.PAmount > _atom.EAmount)
                {
                    _atom.PAmount = _atom.PAmount / 2;
                    _atom.EAmount = _atom.PAmount;
                    _atom.NAmount = _atom.PAmount - 1;
                }
                else if (_atom.PAmount < _atom.EAmount)
                {
                    _atom.EAmount = _atom.EAmount / 2;
                    _atom.PAmount = _atom.EAmount;
                    _atom.NAmount = _atom.EAmount - 1;
                }
                else
                {
                    _atom.EAmount = _atom.NAmount;
                    _atom.PAmount = _atom.NAmount;
                }

                if (_atom.EAmount < 0) _atom.EAmount = 0;
                if (_atom.PAmount < 0) _atom.PAmount = 0;
                if (_atom.NAmount < 0) _atom.NAmount = 0;
            }
            _particleSystem.Play();
            int atomID = _atom.AtomIDUpdate();
            return atomID;
        }

        //decay if player got to many/not enough of neutrons
        private IEnumerator AtomNeutronDecay()
        {
            if (!_inDecay)
            {
                _toDecay = true;
                _inDecay = true;

                //counter before decay
                for (int i = _decayCount; i >= 0;)
                {
                    _audioController.AudioPlay(true, _featuresManager.TicAudio);
                    _animator.SetBool("ToTrim", true);
                    _atomDecayText.text = "Decay in " + i;
                    yield return new WaitForSeconds(1f);
                    i--;

                    //in case if n amount is enough
                    if (_atom.EAmount == _atom.PAmount)
                    {
                        if (_atom.NAmount < _atom.EAmount * 2 && _atom.NAmount >= _atom.EAmount - 2)
                        {
                            _audioController.AudioPlay(false, _featuresManager.TicAudio);
                            _animator.SetBool("ToTrim", false);
                            _toDecay = false;
                            _inDecay = false;
                            _atomDecayText.text = "";
                            break;
                        }
                    }
                }

                _atomDecayText.text = "";

                //in case if player didn't save the atom
                if (_toDecay)
                {
                    if (_atom.NAmount > _atom.EAmount * 2)
                    {
                        _atom.NAmount = _atom.EAmount;

                    }
                    else if (_atom.NAmount <= _atom.EAmount - 2)
                    {
                        _atom.EAmount = _atom.NAmount;
                        _atom.PAmount = _atom.NAmount;
                    }
                    _audioController.AudioPlay(false, _featuresManager.TicAudio);
                    _audioController.AudioPlay(true, _featuresManager.ExplodeAudio);
                    _animator.SetBool("ToTrim", false);
                    _toDecay = false;
                    _inDecay = false;
                    _particleSystem.Play();
                    _atom.AtomID = _atom.AtomIDUpdate();
                    _atom.ScaleScore(1.5f);
                    _atom.CompositionUpdate();
                    DestroyLife();
                }
            }
        }

        //in case if player got event from special particles
        public void HandleSpecialEvent(bool blackHole)
        {
            //if atom hitted by fast neutron
            if (!blackHole)
            {
                int min = Mathf.Min(_atom.EAmount, _atom.NAmount);
                min = Mathf.Min(min, _atom.PAmount);

                _atom.PAmount = min / 2;
                _atom.EAmount = min / 2;
                _atom.NAmount = min / 2;
            }
            //in case if the time of black hole event ended
            else
            {
                if (_atom.PAmount < _atom.EAmount)
                {
                    _atom.PAmount = _atom.EAmount;
                    if (_atom.NAmount <= _atom.PAmount - 2)
                    {
                        _atom.EAmount = _atom.NAmount;
                        _atom.PAmount = _atom.NAmount;
                    }
                }
                else
                {
                    _atom.EAmount = _atom.PAmount;
                    if (_atom.NAmount <= _atom.EAmount - 2)
                    {
                        _atom.EAmount = _atom.NAmount;
                        _atom.PAmount = _atom.NAmount;
                    }
                }
            }
            _particleSystem.Play();
            _audioController.AudioPlay(true, _featuresManager.ExplodeAudio);
            _atom.AtomID = _atom.AtomIDUpdate();
            _atom.CompositionUpdate();
            _atom.AtomUpgrade(_atom.AtomID);
        }

        //lost the live if the atom has been decayed
        private void DestroyLife()
        {
            var livesList = _gameControl.LivesList;
            Object.Destroy(livesList[livesList.Count - 1]);
            livesList.RemoveAt(livesList.Count - 1);

            if (livesList.Count == 0)
            {
                _gameControl.EndGame();
            }
        }
    }
}
