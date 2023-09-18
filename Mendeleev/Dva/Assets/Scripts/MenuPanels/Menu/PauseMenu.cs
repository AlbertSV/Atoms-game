using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dva
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool _gameIsPaused = false;
        public GameObject _pauseMenuUI;
        private Animator _animator;

        private void Start()
        {
            _animator = _pauseMenuUI.GetComponent<Animator>();
        }

        void Update()
        {

            if(_gameIsPaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if(Input.GetKeyUp(KeyCode.Escape))
            {
                if(!_gameIsPaused)
                {

                    Pause();
                }
            }
        }

        public void Resume()
        { 
            _animator.SetBool("Open", false);
            StartCoroutine(WaitForAnimation());
            _gameIsPaused = false;
      

        }

        private void Pause()
        {
            
            _pauseMenuUI.SetActive(true);
            _animator.SetBool("Open", true);
            _gameIsPaused = true;
            
        }

        public void RestartGame()
        {
            _gameIsPaused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(1f);
            _pauseMenuUI.SetActive(false);
        }
    }
}