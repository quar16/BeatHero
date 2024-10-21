using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNodeFunc : MonoBehaviour
{
    public EditorSceneManager ESM;
    public GameObject StartNodeArea;
    public GameObject EndNode;
    public GameObject EndNodeArea;

    public void AreaSet()
    {
        StartNodeArea.transform.localScale = new Vector3(EndNode.transform.localPosition.x, 1, 1);
        EndNodeArea.transform.localScale = new Vector3(EndNode.transform.localPosition.x, 1, 1);
    }
    public void OnDrag()
    {
        float f = Input.mousePosition.x;
        transform.position = new Vector3(f, 590, 0);
        if (ESM.terms != 0)
        {
            float x = transform.localPosition.x;
            float index = 250.0f / Mathf.Pow(2, ESM.terms);
            int a = Mathf.RoundToInt(x / index);
            transform.localPosition = new Vector3(a * index, 0, 0);
        }
        AreaSet();
    }


}
