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
        private Animator _animator;

        void Start()
        {
            _volumeToggle.isOn = (PlayerPrefs.GetInt("volumeToggle") != 0);
            _volumeSlider.value = PlayerPrefs.GetFloat("volumeSlider", 20f);
            _difficultyDropdown.value = PlayerPrefs.GetInt("difficulty");
            _animator = gameObject.GetComponent<Animator>();
            
        }

        //save the settings parameter
        public void Save()
        {
            PlayerPrefs.SetFloat("volumeSlider", _volumeSlider.value);
            PlayerPrefs.SetInt("volumeToggle", (_volumeToggle ? 1 : 0));
            PlayerPrefs.SetInt("difficulty", _difficultyDropdown.value);
        }

        public void Back()
        {
            _animator.SetTrigger("Back");
            
        }    

        private void WaitForAnimationBack()
        {
            _optionMenuUI.transform.localScale = new Vector3(1f, 1f, 1f);
            _optionMenuUI.SetActive(false);
            _mainMenuUI.SetActive(true);
        }

    }
}
