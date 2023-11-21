using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dva
{
    public class ElementsTable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Vector3 _startPosition;
        private GameObject _zoomObject;

        //creating bigger version of the element above the table, when the player pointing at it
        public void OnPointerEnter(PointerEventData eventData)
        {

            _zoomObject = Instantiate(gameObject, new Vector3(0, 17, 95), gameObject.transform.rotation);
            _zoomObject.transform.localScale /= 6f;
        }

        //destroying the bigger version
        public void OnPointerExit(PointerEventData eventData)
        {
            Destroy(_zoomObject);
        }

    }
}