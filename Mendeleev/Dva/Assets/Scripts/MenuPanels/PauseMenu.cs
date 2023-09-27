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

        public void RestartGame()
        {
            Time.timeScale = 1f;
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", false);
            _pauseMenuUI.SetActive(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void MenuGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            //GameObject menu = FindObjectOfType<MainMenu>().gameObject;
            //menu.GetComponent<Animator>().Play("MainMenu");

//            Application.Quit();

//#if UNITY_EDITOR
//            if (UnityEditor.EditorApplication.isPlaying)
//            {
//                UnityEditor.EditorApplication.isPlaying = false;
//            }
//#endif
        }

        private void WaitForAnimationOpen()
        {
            Time.timeScale = 0f;   
        }

        private void WaitForAnimationClose()
        {
            _pauseMenuUI.GetComponent<Animator>().SetTrigger("Close");
            _pauseMenuUI.SetActive(false);
            
        }
    }
}