using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class Atom : MonoBehaviour
    {
        private GameObject _gameManager;
        protected FeaturesManager _featuresManager;
        protected AudioGameController _audioController;
        protected int _atomID;
        protected int _nAmount = 0;
        protected int _eAmount = 1;
        protected int _pAmount = 0;
        protected TMP_Text _atomNameText;
        protected TMP_Text _atomCompositionText;
        protected TMP_Text _atomSymbolText;
        protected TMP_Text _atomSymbolObjectText;
        private TMP_Text _statisticText;
        private Dictionary<int, int> _openElements;
        private int _upgradeCounter = 0;
        private float _multiField = 1.1f;
        private float _multiAmount = 1.5f;
        private int _statistic = 0;
        private Player _player;
        private AtomDecayController _decayController;

        public int AtomID { get => _atomID; internal set => _atomID = value; }
        public int StatisticScore => _statistic;

        internal int NAmount { get => _nAmount; set => _nAmount = value; }
        internal int EAmount { get => _eAmount; set => _eAmount = value; }
        internal int PAmount { get => _pAmount; set => _pAmount = value; }

        void Start()
        {
            _gameManager = FindObjectOfType<GameControl>().gameObject;
            _featuresManager = _gameManager.GetComponent<FeaturesManager>();
            _audioController = _gameManager.GetComponent<AudioGameController>();
            _atomNameText = _featuresManager.AtomName;
            _atomSymbolText = _featuresManager.AtomSymbol;
            _atomCompositionText = _featuresManager.AtomComposition;
            _atomSymbolObjectText = _featuresManager.AtomSymbolObject;
            _statisticText = _featuresManager.StatisticText;
            _openElements = AIUtility.GetPlayerNumbers;
            _player = FindObjectOfType<Player>();
            _decayController = new AtomDecayController(this, _gameManager.GetComponent<GameControl>(), _featuresManager, _audioController);
            _audioController.AudioPlay(true, _featuresManager.StartAudio);
        }

        //check the charge of the atom, if it's not eaqual atom should decay
        protected void AtomCharge()
        {
            _decayController.CheckBalance();
        }

        //in case if player got event from special particles
        public void EventAtomUpdate(bool blackHole)
        {
            _decayController.HandleSpecialEvent(blackHole);
        }

        //add to the player's score and refresh the score text
        internal void AddScore(int amount)
        {
            _statistic += amount;
            StatisticUpdate();
        }

        //shrink the player's score (e.g. after a decay) and refresh the score text
        internal void ScaleScore(float divisor)
        {
            _statistic = (int)(_statistic / divisor);
            StatisticUpdate();
        }

        //updating the text of atom composition after decay
        internal void CompositionUpdate()
        {
            AtomId.Decode(_atomID, out _nAmount, out _eAmount, out _pAmount);
            _atomCompositionText.text = _nAmount + "n" + _eAmount + "e" + _pAmount + "p";
        }

        //upgrade the atom if it has enough amoun of e/p/n
        internal void AtomUpgrade(int atomID)
        {
            AtomId.Decode(atomID, out _, out _, out int level);

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
        internal int AtomIDUpdate()
        {
            _atomID = AtomId.Encode(_nAmount, _eAmount, _pAmount);
            return _atomID;
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
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.y;
            float bottom = _featuresManager.BottomBoarder.transform.position.y;

            _featuresManager.GrowField(_multiField, left, right, top, bottom);

            _gameManager.GetComponent<GameControl>().MaxParticleAmount = (int)(_gameManager.GetComponent<GameControl>().MaxParticleAmount * _multiAmount);
        }

        //update game statistic
        private void StatisticUpdate()
        {
            _statisticText.text = "Score: " + _statistic;
        }

        //change the color of atom
        private void MaterialUpdate(int atomID)
        {
            AtomId.Decode(_atomID, out _, out _, out int elementNumber);
            int materialNumber = AIUtility.GetElementMaterial[elementNumber];

            _player.transform.GetChild(1).GetComponent<SpriteRenderer>().material = _featuresManager.ElementsMaterials[materialNumber - 1];
        }


    }
}