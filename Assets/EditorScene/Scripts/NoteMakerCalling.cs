using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMakerCalling : MonoBehaviour
{

    public EditorSceneManager ESM;
    public GameObject []beatTerms;
    public void Click()
    {
        ESM.StartNodeMake();
    }
    public void TermChange(int terms)
    {
        if (terms == 3)
        {
            beatTerms[0].SetActive(false);
            beatTerms[1].SetActive(false);
            beatTerms[2].SetActive(false);
        }
        else
            beatTerms[terms].SetActive(true);
    }
}
