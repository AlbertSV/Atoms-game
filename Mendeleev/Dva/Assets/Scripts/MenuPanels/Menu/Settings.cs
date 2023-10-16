using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dva
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private Toggle _volumeToggle;
        [SerializeField] private TMP_Dropdown _difficultyDropdown;
        [SerializeField] private GameObject _optionMenuUI;
        [SerializeField] private GameObject _mainMenuUI;
        [SerializeField] private AudioSource _audio;
        private Animator _animator;

        void Start()
        {
            _volumeToggle.isOn = (PlayerPrefs.GetInt("volumeToggle") != 0);
            _difficultyDropdown.value = PlayerPrefs.GetInt("difficulty");
            _animator = gameObject.GetComponent<Animator>();
            SetVolume();
            Debug.Log(_volumeToggle);
            Debug.Log(PlayerPrefs.GetInt("volumeToggle"));

        }

        //save the settings parameter
        public void Save()
        {
            PlayerPrefs.SetInt("volumeToggle", (_volumeToggle.isOn ? 1 : 0));
            PlayerPrefs.SetInt("difficulty", _difficultyDropdown.value);
            Debug.Log(PlayerPrefs.GetInt("volumeToggle"));
            Debug.Log(_volumeToggle.isOn);
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

        public void SetVolume()
        {
            if (_volumeToggle.isOn)
            {
                _audio.Play();
            }
            else
            {
                _audio.Stop();
            }
        }
    }
}
