using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class LevelControl : GameControl
    {
        private int _upgradeCounter = 0;
        private float _multi = 1.5f;

        private void LeveUpgrade()
        {
            int level = (((_atom.AtomID - 1000000000) % 1000000) % 1000);
            Vector3 startField = _featuresManager.Field.localScale;


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

        private void LevelChange()
        {
            Vector3 startField = _featuresManager.Field.localScale;
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.z;
            float bottom = _featuresManager.BottomBoarder.transform.position.z;
           
            _featuresManager.Field.localScale = _featuresManager.Field.localScale * _multi;
            _featuresManager.LeftBoarder.transform.position = new Vector3(left * _multi, _featuresManager.LeftBoarder.transform.position.y, _featuresManager.LeftBoarder.transform.position.z);
            _featuresManager.RightBoarder.transform.position = new Vector3(right * _multi, _featuresManager.RightBoarder.transform.position.y, _featuresManager.RightBoarder.transform.position.z);
            _featuresManager.TopBoarder.transform.position = new Vector3(_featuresManager.TopBoarder.transform.position.x, _featuresManager.TopBoarder.transform.position.y, top * _multi);
            _featuresManager.BottomBoarder.transform.position = new Vector3(_featuresManager.BottomBoarder.transform.position.x, _featuresManager.BottomBoarder.transform.position.y, bottom * _multi);
            _maxParticleAmount = (int)(_maxParticleAmount * _multi);
        }
    }
}