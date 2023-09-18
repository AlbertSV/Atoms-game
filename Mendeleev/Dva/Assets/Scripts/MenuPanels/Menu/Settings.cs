using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dva
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Toggle _volumeToggle;
        [SerializeField] private TMP_Dropdown _difficultyDropdown;
        [SerializeField] private GameObject _optionMenuUI;
        [SerializeField] private GameObject _mainMenuUI;

        void Start()
        {
            _volumeToggle.isOn = (PlayerPrefs.GetInt("volumeToggle") != 0);
            _volumeSlider.value = PlayerPrefs.GetFloat("volumeSlider", 20f);
            _difficultyDropdown.value = PlayerPrefs.GetInt("difficulty");
            
        }

        //save the settings parameters
        public void Save()
        {
            PlayerPrefs.SetFloat("volumeSlider", _volumeSlider.value);
            PlayerPrefs.SetInt("volumeToggle", (_volumeToggle ? 1 : 0));
            PlayerPrefs.SetInt("difficulty", _difficultyDropdown.value);
        }

        public void Back()
        {
            WaitForAnimation();
            _mainMenuUI.SetActive(true);
        }    

        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(0.6f);
            _optionMenuUI.SetActive(false);
        }

    }
}
