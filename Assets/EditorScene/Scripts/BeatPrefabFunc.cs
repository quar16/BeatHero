using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatPrefabFunc : MonoBehaviour
{
    public GameObject copy;
    public int beatType;
    public BeatMenuFunc BeatMenu;
    public int summonPos = 0;
    public void Click()
    {
        BeatMenu.gameObject.SetActive(true);
        BeatMenu.PositionSet(summonPos);
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
