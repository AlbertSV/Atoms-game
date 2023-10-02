using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Dva
{
    public class Progress : MonoBehaviour
    {
        private Dictionary<int, int> _openElemenets;
        private Dictionary<string, GameObject> _elements;

        private void Awake()
        {
            _openElemenets = new Dictionary<int, int>();
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

                if (!_openElemenets.ContainsKey(atomID))
                {
                    _openElemenets[atomID] = number;
                }

            }
        }

        private void OpenElemets()
        {
            var listOfAtoms = AIUtility.GetPlayerNumbers.GroupBy(x => x.Value).Select(grp => grp.ToList()).ToList();

            foreach (List<KeyValuePair<int,int>> element in listOfAtoms)
            {
                Debug.Log(element);
                //int.Parse(element);
                //_elements[element].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //Debug.Log(_elements[element]);
            }

        }

    }
}