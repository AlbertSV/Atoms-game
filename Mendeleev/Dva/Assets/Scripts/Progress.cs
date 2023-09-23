using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class Progress : MonoBehaviour
    {
        private List<int> _openElemenets;

        private void Awake()
        {
            _openElemenets = new List<int>();
        }

        public void CheckAtomID(int atomID)
        {
            if(AIUtility.GetAtomName.ContainsKey(atomID))
            {
                int number = (((atomID - 1000000000) % 1000000) % 1000);
                int nAmount = (atomID - 1000000000) / 1000000;

                if (!_openElemenets.Contains(number))
                {
                    _openElemenets.Add(number);
                }


            }
        }
    }
}