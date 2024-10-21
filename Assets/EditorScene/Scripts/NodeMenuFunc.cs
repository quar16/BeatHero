using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMenuFunc : MonoBehaviour
{
    public GameObject Node;

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
    public void Delete()
    {
        Destroy(Node);
    }
}
