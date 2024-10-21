using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartNodeMoveHandle : MonoBehaviour, IDragHandler
{
    public StartNodeFunc snf;
    public GameObject NodeMenu;
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        snf.OnDrag();
    }
    public void Click()
    {
        NodeMenu.SetActive(true);
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
