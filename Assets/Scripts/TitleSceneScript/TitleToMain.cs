using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToMain : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey)
        {
            Bridge.SceneCall(Scene.Title, Scene.Main);
        }
    }
}
