using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RepeatDialLeft : MonoBehaviour, IDragHandler
{
    public GameObject RepeatDialRight;
    public EditorSceneManager ESM;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x - 880;
        f = Mathf.Clamp(f, -825, RepeatDialRight.transform.localPosition.x - 10);
        transform.localPosition = new Vector3(f, -15, 0);
        ESM.repeatStartD = (f + 825) / 1650;
    }
}
