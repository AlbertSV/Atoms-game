using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class Atom : MonoBehaviour
    {
        private GameObject _gameManager;
        private int _decayCount;
        private bool _toDecay = false;
        private bool _inDecay = false;
        protected int _atomID;
        protected int _nAmount = 0;
        protected int _eAmount = 1;
        protected int _pAmount = 0;
        protected TMP_Text _atomNameText;
        protected TMP_Text _atomCompositionText;
        protected TMP_Text _atomSymbolText;
        protected TMP_Text _atomSymbolObjectText;
        protected TMP_Text _atomDecayText;
        private ParticleSystem _particleSystem;
        private Animator _animator;
        private List<string> _openElements;

        public int AtomID => _atomID;

        void Start()
        {
            _gameManager = FindObjectOfType<GameControl>().gameObject;
            _decayCount = _gameManager.GetComponent<FeaturesManager>().DecayCountDown;
            _atomNameText = _gameManager.GetComponent<FeaturesManager>().AtomName;
            _atomSymbolText = _gameManager.GetComponent<FeaturesManager>().AtomSymbol;
            _atomCompositionText = _gameManager.GetComponent<FeaturesManager>().AtomComposition;
            _atomDecayText = _gameManager.GetComponent<FeaturesManager>().AtomDecay;
            _atomSymbolObjectText = _gameManager.GetComponent<FeaturesManager>().AtomSymbolObject;
            _particleSystem = _gameManager.GetComponent<FeaturesManager>().ParticleSystem;
            _animator = _particleSystem.GetComponentInParent<Animator>();
            _openElements = AIUtility.GetPlayerNumbers;
        }


        protected void AtomCharge()
        {
            if (_eAmount == _pAmount)
            {
                if (_nAmount > _eAmount * 2)
                {
                    StartCoroutine(AtomNeutronDecay());
                }
                else if(_nAmount <= _eAmount - 2)
                {
                    StartCoroutine(AtomNeutronDecay());
                }

                AtomUpgrade(_atomID);
                CompositionUpdate();
            }
            else
            {
                if (_nAmount == 1 && _eAmount == 1 && _pAmount == 0 || _nAmount == 1 && _eAmount == 0 && _pAmount == 1)
                {
                    CompositionUpdate();
                }
                else
                {
                    StartCoroutine(ParticleStateAmountDecrease());
                }
            }
        }
        private IEnumerator ParticleStateAmountDecrease()
        {
            if (!_inDecay)
            {
                _toDecay = true;
                _inDecay = true;

                for (int i = _decayCount; i >= 0;)
                {
                    _atomDecayText.text = "Decay in " + i;
                    _animator.SetBool("ToTrim", true);
                    yield return new WaitForSeconds(1f);
                    i--;
                    if (_eAmount == _pAmount && _nAmount >= _eAmount - 2)
                    {
                        _toDecay = false;
                        _animator.SetBool("ToTrim", false);
                        _atomDecayText.text = "";
                        CompositionUpdate();
                        _inDecay = false;
                        break;
                    }
                }
                _atomDecayText.text = "";
                if (_toDecay)
                {
                    _animator.SetBool("ToTrim", false);
                    _particleSystem.Play();
                    _atomID = AtomDecay();
                    AtomUpgrade(_atomID);
                    CompositionUpdate();
                    _inDecay = false;
                    _toDecay = false;
                }
            }
        }


        private int AtomDecay()
        {
            {
                if (_pAmount > _eAmount)
                {
                    _pAmount = _pAmount / 2;
                    _eAmount = _pAmount-1;
                    _nAmount = _pAmount-1;
                }
                else if(_pAmount < _eAmount)
                {
                    _eAmount = _eAmount / 2;
                    _pAmount = _eAmount-1;
                    _nAmount = _eAmount-1;
                }
                else
                {
                    _eAmount = _nAmount;
                    _pAmount = _nAmount;
                }

                if (_eAmount < 0) _eAmount = 0;
                if (_pAmount < 0) _pAmount = 0;
                if (_nAmount < 0) _nAmount = 0;
            }
            _particleSystem.Play();
            int atomID = AtomIDUpdate();
            return atomID;
        }

        protected IEnumerator AtomNeutronDecay()
        {
            if (!_inDecay)
            {
                _toDecay = true;
                _inDecay = true;

                for (int i = _decayCount; i >= 0;)
                {
                    _animator.SetBool("ToTrim", true);
                    _atomDecayText.text = "Decay in " + i;
                    yield return new WaitForSeconds(1f);
                    i--;
                    if (_eAmount == _pAmount)
                    {
                        if (_nAmount < _eAmount * 2 && _nAmount >= _eAmount - 2)
                        {
                            _toDecay = false;
                            _inDecay = false;
                            _atomDecayText.text = "";
                            break;
                        }
                    }
                }

                _atomDecayText.text = "";

                if (_toDecay)
                {
                    if (_nAmount > _eAmount * 2)
                    {
                        _nAmount = _eAmount;

                    }
                    else if (_nAmount <= _eAmount - 2)
                    {
                        _eAmount = _nAmount;
                        _pAmount = _nAmount;
                    }
                    _animator.SetBool("ToTrim", false);
                    _toDecay = false;
                    _inDecay = false;
                    _particleSystem.Play();
                    _atomID = AtomIDUpdate();
                    CompositionUpdate();
                }
            }
        }

        public void EventAtomUpdate(bool blackHole)
        {
            if(!blackHole)
            {
                int min = Math.Min(_eAmount, _nAmount);
                min = Math.Min(min, _pAmount);

                _pAmount = min / 2;
                _eAmount = min / 2;
                _nAmount = min / 2;
            }
            else
            {
                if(_pAmount < _eAmount)
                {
                    _pAmount = _eAmount;
                    if(_nAmount <= _pAmount - 2)
                    {
                        _eAmount = _nAmount;
                        _pAmount = _nAmount;
                    }
                }
                else
                {
                    _eAmount = _pAmount;
                    if (_nAmount <= _eAmount - 2)
                    {
                        _eAmount = _nAmount;
                        _pAmount = _nAmount;
                    }
                }
            }
            _particleSystem.Play();
            _atomID = AtomIDUpdate();
            CompositionUpdate();
            AtomUpgrade(_atomID);
        }

        protected void CompositionUpdate()
        {
            _nAmount = (_atomID - 1000000000) / 1000000;
            _eAmount = ((_atomID - 1000000000) % 1000000) / 1000;
            _pAmount = (((_atomID - 1000000000) % 1000000) % 1000);
            _atomCompositionText.text = _nAmount + "n" + _eAmount + "e" + _pAmount + "p";
        }

        protected void AtomUpgrade(int atomID)
        {
            string number = (((atomID - 1000000000) % 1000000) % 1000).ToString();

            if (AIUtility.GetAtomName.ContainsKey(atomID))
            {
                string name = AIUtility.GetAtomName[atomID];
                _atomNameText.text = name;
            }
            else
            {
                _atomNameText.text = "Unknown";
            }

            if (AIUtility.GetAtomSymbol.ContainsKey(atomID))
            {
                string symbol = AIUtility.GetAtomSymbol[atomID];
                _atomSymbolText.text = symbol;
                _atomSymbolObjectText.text = symbol;
                if (!_openElements.Contains(number))
                {
                    _openElements.Add(number);
                    AIUtility.RewriteXML(number);
                }
            }
            else
            {
                _atomSymbolText.text = "X";
                _atomSymbolObjectText.text = "X";
            }

        }

        protected int AtomIDUpdate()
        {
            _atomID = 1000000000 + _nAmount * 1000000 + _eAmount * 1000 + _pAmount;
            return _atomID;
        }
    }
}