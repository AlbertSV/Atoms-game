using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class Progress : MonoBehaviour
    {
        private Dictionary<int, int> _openElemenets;
        private Dictionary<string, GameObject> _elements;
        [SerializeField] private TMP_Text _score;

        private void Awake()
        {
            _openElemenets = new Dictionary<int, int>();
            _elements = new Dictionary<string, GameObject>();
            ElementsTable[] allElements = FindObjectsOfType<ElementsTable>();
            _elements = allElements.ToDictionary(v => v.gameObject.name, v => v.gameObject);
            OpenElemets();
            Statistic();

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
            var listOfAtoms = AIUtility.GetPlayerNumbers.GroupBy(x => x.Value).Select(x => new
            {
                Atom = x.Key,
                Count = x.Count()
            });
            

            foreach (var isotop in listOfAtoms)
            {
                string element = isotop.Atom.ToString();
                _elements[element].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                GameObject isotopChild = _elements[element].transform.GetChild(4).gameObject;
                int totalIsotopes = AIUtility.GetElementIsotopes[isotop.Atom];
                isotopChild.GetComponent<TMP_Text>().text = "Isotopes: " + isotop.Count + "/" + totalIsotopes;
            }

        }

        private void Statistic()
        {
            int score  = PlayerPrefs.GetInt("Statistic");

            _score.text = "Score: " + score;
        }

    }
}