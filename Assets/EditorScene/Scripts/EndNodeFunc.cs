using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndNodeFunc : MonoBehaviour, IDragHandler
{
    public StartNodeFunc SNF;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x;
        f = Mathf.Clamp(f, SNF.transform.position.x, 1920);
        transform.parent.position = new Vector3(f, 590, 0);
        if (SNF.ESM.terms != 0)
        {
            float x = transform.parent.localPosition.x;
            float index = 250.0f / Mathf.Pow(2, SNF.ESM.terms);
            int a = Mathf.RoundToInt(x / index);
            transform.parent.localPosition = new Vector3(a * index, 0, 0);
        }
        SNF.AreaSet();
    }
    public AudioSource AS;

    float Pos = 0;
    int inFrame = 0;

    private void Update()
    {
        Pos = transform.position.x;
        if (Pos < BXC.BX && BXC.BX < Pos + 20 && BXC.isPlaying)
        {
            if (inFrame == 0)
            {
                inFrame = 1;
                AS.Play();
            }
        }
        else if (inFrame != 0)
        {
            inFrame++;
            if (inFrame == 30)
                inFrame = 0;
        }
    }
}
