using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RepeatDialRight : MonoBehaviour, IDragHandler
{
    public GameObject RepeatDialLeft;
    public EditorSceneManager ESM;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x - 880;
        f = Mathf.Clamp(f, RepeatDialLeft.transform.localPosition.x + 10, 825);
        transform.localPosition = new Vector3(f, -15, 0);
        ESM.repeatEndD = (f + 825) / 1650;
    }
}
