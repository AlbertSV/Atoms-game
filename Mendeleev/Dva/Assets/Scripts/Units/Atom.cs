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
        protected FeaturesManager _featuresManager;
        protected AudioGameController _audioController;
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
        private TMP_Text _statisticText;
        private ParticleSystem _particleSystem;
        private Animator _animator;
        private Dictionary<int, int> _openElements;
        private int _upgradeCounter = 0;
        private float _multiField = 1.1f;
        private float _multiAmount = 1.5f;
        private int _statistic = 0;
        private Player _player;

        public int AtomID => _atomID;
        public int StatisticScore => _statistic;


        void Start()
        {
            _gameManager = FindObjectOfType<GameControl>().gameObject;
            _featuresManager = _gameManager.GetComponent<FeaturesManager>();
            _audioController = _gameManager.GetComponent<AudioGameController>();
            _decayCount = _featuresManager.DecayCountDown;
            _atomNameText = _featuresManager.AtomName;
            _atomSymbolText = _featuresManager.AtomSymbol;
            _atomCompositionText = _featuresManager.AtomComposition;
            _atomDecayText = _featuresManager.AtomDecay;
            _atomSymbolObjectText = _featuresManager.AtomSymbolObject;
            _particleSystem = _featuresManager.ParticleSystem;
            _statisticText = _featuresManager.StatisticText;
            _animator = _particleSystem.GetComponentInParent<Animator>();
            _openElements = AIUtility.GetPlayerNumbers;
            _player = FindObjectOfType<Player>();
            _audioController.AudioPlay(true, _featuresManager.StartAudio);
        }


        //check the charge of the atom, if it's not eaqual atom should decay
        protected void AtomCharge()
        {
            if (_eAmount == _pAmount)
            {
                if (_nAmount > _eAmount * 2)
                {
                    StartCoroutine(AtomNeutronDecay());
                }
                else if (_nAmount <= _eAmount - 2)
                {
                    StartCoroutine(AtomNeutronDecay());
                }

                AtomUpgrade(_atomID);
                CompositionUpdate();
                _statistic += 10;
                StatisticUpdate();
            }
            else
            {
                if (_nAmount == 1 && _eAmount == 1 && _pAmount == 0 || _nAmount == 1 && _eAmount == 0 && _pAmount == 1)
                {
                    CompositionUpdate();
                    _statistic += 10;
                    StatisticUpdate();
                }
                else
                {
                    StartCoroutine(ParticleStateAmountDecrease());
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
                    if (_eAmount == _pAmount && _nAmount >= _eAmount - 2)
                    {
                        _audioController.AudioPlay(false, _featuresManager.TicAudio);
                        _toDecay = false;
                        _animator.SetBool("ToTrim", false);
                        _atomDecayText.text = "";
                        CompositionUpdate();
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
                    _atomID = AtomDecay();
                    AtomUpgrade(_atomID);
                    CompositionUpdate();
                    _statistic = (int)(_statistic / 2);
                    StatisticUpdate();
                    DestroyLife(_gameManager.GetComponent<GameControl>()._livesList);
                    _inDecay = false;
                    _toDecay = false;
                }
            }
        }

        //set the amount of particles after decay
        private int AtomDecay()
        {
            {
                if (_pAmount > _eAmount)
                {
                    _pAmount = _pAmount / 2;
                    _eAmount = _pAmount - 1;
                    _nAmount = _pAmount - 1;
                }
                else if (_pAmount < _eAmount)
                {
                    _eAmount = _eAmount / 2;
                    _pAmount = _eAmount - 1;
                    _nAmount = _eAmount - 1;
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

        //decay if player got to many/not enough of neutrons
        protected IEnumerator AtomNeutronDecay()
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
                    if (_eAmount == _pAmount)
                    {
                        if (_nAmount < _eAmount * 2 && _nAmount >= _eAmount - 2)
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
                    if (_nAmount > _eAmount * 2)
                    {
                        _nAmount = _eAmount;

                    }
                    else if (_nAmount <= _eAmount - 2)
                    {
                        _eAmount = _nAmount;
                        _pAmount = _nAmount;
                    }
                    _audioController.AudioPlay(false, _featuresManager.TicAudio);
                    _audioController.AudioPlay(true, _featuresManager.ExplodeAudio);
                    _animator.SetBool("ToTrim", false);
                    _toDecay = false;
                    _inDecay = false;
                    _particleSystem.Play();
                    _atomID = AtomIDUpdate();
                    _statistic = (int)(_statistic / 1.5);
                    StatisticUpdate();
                    CompositionUpdate();
                    DestroyLife(_gameManager.GetComponent<GameControl>()._livesList);
                }
            }
        }

        //in case if player got event from special particles
        public void EventAtomUpdate(bool blackHole)
        {
            //if atom hitted by fast neutron
            if (!blackHole)
            {
                int min = Math.Min(_eAmount, _nAmount);
                min = Math.Min(min, _pAmount);
                
                _pAmount = min / 2;
                _eAmount = min / 2;
                _nAmount = min / 2;
            }
            //in case if the time of black hole event ended
            else
            {
                if (_pAmount < _eAmount)
                {
                    _pAmount = _eAmount;
                    if (_nAmount <= _pAmount - 2)
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
            _audioController.AudioPlay(true, _featuresManager.ExplodeAudio);
            _atomID = AtomIDUpdate();
            CompositionUpdate();
            AtomUpgrade(_atomID);
        }

        //updating the text of atom composition after decay
        protected void CompositionUpdate()
        {
            _nAmount = (_atomID - 1000000000) / 1000000;
            _eAmount = ((_atomID - 1000000000) % 1000000) / 1000;
            _pAmount = (((_atomID - 1000000000) % 1000000) % 1000);
            _atomCompositionText.text = _nAmount + "n" + _eAmount + "e" + _pAmount + "p";
        }

        //upgrade the atom if it has enough amoun of e/p/n
        protected void AtomUpgrade(int atomID)
        {
            int level = ((atomID - 1000000000) % 1000000) % 1000;

            LeveUpgrade(level);

            //get the name of new atom, if it's exist in the element table
            if (AIUtility.GetAtomName.ContainsKey(atomID))
            {
                string name = AIUtility.GetAtomName[atomID];
                _atomNameText.text = name;
                _statistic += 100;
                
                StatisticUpdate();
                MaterialUpdate(atomID);
            }
            else
            {
                _atomNameText.text = "Unknown";

                _player.transform.GetChild(1).GetComponent<SpriteRenderer>().material = _featuresManager.ElementsMaterials[9];
            }

            //if this atom discovered for the first time by this player
            if (AIUtility.GetAtomSymbol.ContainsKey(atomID))
            {
                string symbol = AIUtility.GetAtomSymbol[atomID];
                _atomSymbolText.text = symbol;
                _atomSymbolObjectText.text = symbol;

                if (!_openElements.ContainsKey(atomID))
                {
                    _statistic = (int)(_statistic * 1.3f);
                    StatisticUpdate();
                    _openElements.Add(atomID, level);
                    AIUtility.RewriteXML(level, atomID);
                }

            }
            else
            {
                _atomSymbolText.text = "X";
                _atomSymbolObjectText.text = "X";
            }

        }

        //update current atom ID
        protected int AtomIDUpdate()
        {
            _atomID = 1000000000 + _nAmount * 1000000 + _eAmount * 1000 + _pAmount;
            return _atomID;
        }

        //lost the live if the atom has been decayed
        private void DestroyLife(List<GameObject> lifesList)
        {

            Destroy(lifesList[lifesList.Count - 1]);
            lifesList.RemoveAt(lifesList.Count - 1);

            if (lifesList.Count == 0)
            {
                _gameManager.GetComponent<GameControl>().EndGame();
            }
        }

        //check if the player increase atom level
        private void LeveUpgrade(int level)
        {
            if (level > 20 && _upgradeCounter == 0)
            {
                LevelChange();
                _upgradeCounter = 1;
            }
            else if (level > 40 && _upgradeCounter == 1)
            {
                LevelChange();
                _upgradeCounter = 2;
            }
            else if (level > 60 && _upgradeCounter == 2)
            {
                LevelChange();
                _upgradeCounter = 3;
            }
            else if (level > 80 && _upgradeCounter == 3)
            {
                LevelChange();
                _upgradeCounter = 4;
            }
            else if (level > 100 && _upgradeCounter == 4)
            {
                LevelChange();
                _upgradeCounter = 5;
            }
            else if (level > 110 && _upgradeCounter == 5)
            {
                LevelChange();
                _upgradeCounter = 6;
            }
        }

        //increase the field and particles amount after atom's new level
        private void LevelChange()
        {
            Vector3 startField = _featuresManager.Field.localScale;
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.y;
            float bottom = _featuresManager.BottomBoarder.transform.position.y;

            _featuresManager.Field.localScale = new Vector3(startField.x * _multiField, startField.y, startField.z * _multiField);

            _featuresManager.LeftBoarder.transform.position = new Vector3(left * _multiField, _featuresManager.LeftBoarder.transform.position.y, _featuresManager.LeftBoarder.transform.position.z);
            _featuresManager.LeftBoarder.transform.localScale = new Vector3(_featuresManager.Field.localScale.z, _featuresManager.LeftBoarder.localScale.y, _featuresManager.LeftBoarder.localScale.z);

            _featuresManager.RightBoarder.transform.position = new Vector3(right * _multiField, _featuresManager.RightBoarder.transform.position.y, _featuresManager.RightBoarder.transform.position.z);
            _featuresManager.RightBoarder.transform.localScale = new Vector3(_featuresManager.Field.localScale.z, _featuresManager.RightBoarder.localScale.y, _featuresManager.RightBoarder.localScale.z);

            _featuresManager.TopBoarder.transform.position = new Vector3(_featuresManager.TopBoarder.transform.position.x, top * _multiField, _featuresManager.TopBoarder.transform.position.z);
            _featuresManager.TopBoarder.transform.localScale = new Vector3(_featuresManager.TopBoarder.localScale.x, _featuresManager.Field.localScale.x, _featuresManager.TopBoarder.localScale.z);

            _featuresManager.BottomBoarder.transform.position = new Vector3(_featuresManager.BottomBoarder.transform.position.x, bottom * _multiField, _featuresManager.BottomBoarder.transform.position.y);
            _featuresManager.BottomBoarder.transform.localScale = new Vector3(_featuresManager.BottomBoarder.localScale.x, _featuresManager.Field.localScale.x, _featuresManager.BottomBoarder.localScale.z);

            _gameManager.GetComponent<GameControl>()._maxParticleAmount = (int)(_gameManager.GetComponent<GameControl>()._maxParticleAmount * _multiAmount);
        }

        //update game statistic
        private void StatisticUpdate()
        {
            _statisticText.text = "Score: " + _statistic;
        }

        //change the color of atom
        private void MaterialUpdate(int atomID)
        {
            int elementNumber = (((_atomID - 1000000000) % 1000000) % 1000);
            int materialNumber = AIUtility.GetElementMaterial[elementNumber];

            _player.transform.GetChild(1).GetComponent<SpriteRenderer>().material = _featuresManager.ElementsMaterials[materialNumber - 1];
        }


    }
}