using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSceneSetting : MonoBehaviour
{
    public EditorSceneManager ESM;
    [Header("-------------------------------------------------------------------------------------")]
    [Space(30)]
    public AudioClip Music;
    public int BPM;
    public float Offset;
    public BeatData beatData;

    private void Update()
    {
        ESM.Offset = Offset;
        ESM.OffsetT.text = "오프셋 | " + Offset.ToString();
    }
}
