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

        public void OnPointerEnter(PointerEventData eventData)
        {
            //transform.localScale *= 5f;
            //_startPosition = transform.localPosition;
            //transform.localPosition = new Vector3(0, 320, 0);

            _zoomObject = Instantiate(gameObject, new Vector3(0, 17, 95), gameObject.transform.rotation);
            _zoomObject.transform.localScale /= 6f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //transform.localScale /= 5f;
            //transform.localPosition = _startPosition;
            Destroy(_zoomObject);
        }

    }
}