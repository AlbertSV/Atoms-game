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
        [SerializeField] private GameObject _elementMenuUI;
        private Animator _animator;

        private void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
            _animator.Play("MainMenu");
        }

        public void PlayGame()
        {
            if(_animator != null)
            {
                _animator.SetTrigger("ToPlay");
            }

        }

        public void ElementMenu()
        {
            _mainMenuUI.SetActive(false);
            _elementMenuUI.SetActive(true);
            _elementMenuUI.GetComponent<Animator>().Play("ElementMenu");
        }

        public void OptionMenu()
        {
            _mainMenuUI.SetActive(false);
            _optionMenuUI.SetActive(true);
            _optionMenuUI.GetComponent<Animator>().Play("OptionMenuOpen");
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