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


        public void PlayGame()
        {
            StartCoroutine(WaitForAnimation());
            
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

        private IEnumerator WaitForAnimation()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}