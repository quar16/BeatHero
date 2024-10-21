using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSound : MonoBehaviour
{
    public AudioSource AS;

    float Pos = 0;
    int delay = 0;
    int inFrame = 0;
    float id = 0;
    private void Start()
    {
        id = transform.position.x;
    }
    private void Update()
    {
        inFrame++;
        Pos = transform.position.x;
        if (Pos < BXC.BX && BXC.BX < Pos + 20 && BXC.isPlaying)
        {
            if (delay == 0)
            {
                delay = 1;
                Debug.Log("ID : " + id + " | 프레임 : " + inFrame + " | 위치 : " + Pos);
                AS.Play();
            }
        }
        else if (delay != 0)
        {
            delay++;
            if (delay == 5)
                delay = 0;
        }
    }
}
