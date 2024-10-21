using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongNotePosHandle : MonoBehaviour, IDragHandler
{
    public EditorSceneManager ESM;
    public GameObject SizeHandle;
    public GameObject Area;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float f = Input.mousePosition.x;
        transform.parent.position = new Vector3(f, 590, 1);
        if (ESM.terms != 0)
        {
            float x = transform.parent.localPosition.x;
            float index = 250.0f / Mathf.Pow(2, ESM.terms);
            int a = Mathf.RoundToInt(x / index);
            transform.parent.localPosition = new Vector3(a * index, 0, 1);
        }
        AreaSet();
    }
    public void AreaSet()
    {
        Area.transform.localScale = new Vector3(SizeHandle.transform.localPosition.x, 1, 1);
    }
    public GameObject Menu;
    public void Click()
    {
        Menu.SetActive(true);
    }

    public AudioSource AS;

    float Pos1 = 0;
    float Pos2 = 0;
    float inFrame = 0;

    private void Update()
    {
        Pos1 = transform.position.x;
        Pos2 = SizeHandle.transform.position.x;
        if (Pos1 < BXC.BX && BXC.BX < Pos2 && BXC.isPlaying)
        {
            if (inFrame > 0.125f)
            {
                inFrame = 0;
                AS.Play();
            }
            inFrame += Time.deltaTime;
        }
    }
}
