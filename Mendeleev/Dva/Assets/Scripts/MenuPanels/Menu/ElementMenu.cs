using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class ElementMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _elementMenuUI;
        [SerializeField] private GameObject _mainMenuUI;
        private Animator _animator;

        void Start()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void ElementBack()
        {
            _animator.SetTrigger("Back");
        }

        //getting back to main menu after button clicked
        private void AnimationElementBack()
        {
            _elementMenuUI.transform.localScale = new Vector3(1f, 1f, 1f);
            _elementMenuUI.SetActive(false);
            _mainMenuUI.SetActive(true);
        }
    }
}