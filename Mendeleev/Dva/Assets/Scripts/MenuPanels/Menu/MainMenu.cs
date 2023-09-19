using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;

namespace Dva
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuUI;
        [SerializeField] private GameObject _optionMenuUI;
        private Animator _animator;

        private void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void PlayGame()
        {
            if(_animator != null)
            {
                _animator.SetTrigger("ToPlay");
            }

        }

        public void OptionMenu()
        {
            _mainMenuUI.SetActive(false);
            _optionMenuUI.SetActive(true);
        }    

        public void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }

        private void WaitForAnimationPlay()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}