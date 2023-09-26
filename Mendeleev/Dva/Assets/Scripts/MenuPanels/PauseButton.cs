using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenuUI;

        public void Pause()
        {
            _pauseMenuUI.SetActive(true);
            _pauseMenuUI.GetComponent<Animator>().SetBool("Open", true);
        }
    }
}