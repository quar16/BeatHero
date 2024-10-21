using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNodeCreateHandle : MonoBehaviour
{
    public StartNodeFunc SNF;
    public void Click()
    {
        SNF.ESM.NoteMake(SNF.transform);
    }
}
