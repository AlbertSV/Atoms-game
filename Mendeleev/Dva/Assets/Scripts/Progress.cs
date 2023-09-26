using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Dva
{
    public class Progress : MonoBehaviour
    {
        private List<int> _openElemenets;
        private Dictionary<string, GameObject> _elements;

        private void Awake()
        {
            _openElemenets = new List<int>();
            _elements = new Dictionary<string, GameObject>();
            ElementsTable[] allElements = FindObjectsOfType<ElementsTable>();
            _elements = allElements.ToDictionary(v => v.gameObject.name, v => v.gameObject);
            OpenElemets();

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

        private void OpenElemets()
        {
            foreach (string element in (AIUtility.GetPlayerNumbers))
            {
                _elements[element].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Debug.Log(_elements[element]);
            }

        }

    }
}