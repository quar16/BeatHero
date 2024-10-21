using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMenuFunc : MonoBehaviour
{
    public BeatPrefabFunc Beat;
    public Image[] SummonPosButtons;

    public void Cancel() {
        gameObject.SetActive(false);
    }
    public void Delete() {
        if (Beat.copy != null)
        {
            Destroy(Beat.copy);
            Destroy(Beat.gameObject);
        }
    }
    public void PositionSet(int p) {
        Beat.summonPos = p;
        for (int i = 0; i < 3; i++)
            SummonPosButtons[i].color = new Color(0.6f, 0.6f, 0.6f);
        if (p != 0)
            SummonPosButtons[p - 1].color = new Color(0.3f, 0.3f, 0.3f);
    }
}
