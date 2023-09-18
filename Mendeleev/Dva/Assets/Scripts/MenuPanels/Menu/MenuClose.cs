using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    public class MenuClose : StateMachineBehaviour
    {
        public GameObject _menuPanelUI;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _menuPanelUI.SetActive(false);
        }
  
    }
}