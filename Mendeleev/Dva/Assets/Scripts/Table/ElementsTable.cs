using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dva
{
    public class ElementsTable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {


        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale *= 2f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale /= 2f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
           
        }

    }
}