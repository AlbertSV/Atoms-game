using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dva
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenuUI;

        //void Update()
        //{

        //    if(_gameIsPaused)
        //    {
        //        Time.timeScale = 0f;
        //    }
        //    else
        //    {
        //        Time.timeScale = 1f;
        //    }

        //    if(Input.GetKeyUp(KeyCode.Escape))
        //    {
        //        if(!_gameIsPaused)
        //        {

        //            Pause();
        //        }
        //    }
        //}

        public void Resume()
        {
            _pauseMenuUI.GetComponent<Animator>().SetTrigger("Close");
        }

        //public void Pause()
        //{
        //    _pauseMenuUI.GetComponent<Animator>().Play("PauseMenuOpen");
        //}

        public void RestartGame()
        {
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", false);
            _pauseMenuUI.SetActive(false);
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

        private void WaitForAnimationOpen()
        {
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", false);
            Time.timeScale = 0f;
            
        }

        private void WaitForAnimationClose()
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
           
        }
    }
}