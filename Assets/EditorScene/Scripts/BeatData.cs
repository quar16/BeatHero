using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BeatNode
{
    public float NodeStartTime;
    public float NodeEndTime;
    public bool isLong;
    public Beat[] beats;
}

[System.Serializable]
public struct Beat
{
    public float BeatTime;
    public int Type;
    public int Direction;
    public int SummonPos;
}

[System.Serializable]
public class BeatData : MonoBehaviour
{
    public int musicID;
    public int BPM;
    public float Offset;
    public BeatNode[] beatNodes;
}
