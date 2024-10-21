using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongNoteSizeHandle : MonoBehaviour, IDragHandler
{
    public LongNotePosHandle PosHandle;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x;
        f = Mathf.Clamp(f, PosHandle.transform.position.x, 1920);
        transform.position = new Vector3(f, 590, 1);
        if (PosHandle.ESM.terms != 0)
        {
            float x = transform.localPosition.x;
            float index = 250.0f / Mathf.Pow(2, PosHandle.ESM.terms);
            int a = Mathf.RoundToInt(x / index);
            transform.localPosition = new Vector3(a * index, 0, 1);
        }
        PosHandle.AreaSet();
    }
}
