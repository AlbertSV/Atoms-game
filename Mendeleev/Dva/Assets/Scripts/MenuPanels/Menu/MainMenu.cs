using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        //going to elements menu
        public void ElementMenu()
        {
            _mainMenuUI.SetActive(false);
            _elementMenuUI.SetActive(true);
            _elementMenuUI.GetComponent<Animator>().Play("ElementMenu");
        }

        //going to option menu
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

        //loading synchronously blocks the main thread until the whole scene is ready, which is
        //exactly the stutter on "Play" - async loading keeps the app responsive while it loads
        private void WaitForAnimationPlay()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}