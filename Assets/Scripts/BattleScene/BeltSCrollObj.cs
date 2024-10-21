using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSCrollObj : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x < -200 * 1.3f)
        {
            transform.position += Vector3.right * (576 * 1.3f);
        }
    }
}
