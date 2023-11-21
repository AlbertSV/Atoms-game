using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dva
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenuUI;

        public void Resume()
        {
            Time.timeScale = 1f;
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", false);
        }

        //restarting game from the beginning
        public void RestartGame()
        {
            Time.timeScale = 1f;
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", false);
            _pauseMenuUI.SetActive(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //getting into main menu
        public void MenuGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        }

        //event after game put on pause
        private void WaitForAnimationOpen()
        {
            Time.timeScale = 0f;   
        }

        //event after game started again
        private void WaitForAnimationClose()
        {
            _pauseMenuUI.GetComponent<Animator>().SetTrigger("Close");
            _pauseMenuUI.SetActive(false);
            
        }
    }
}