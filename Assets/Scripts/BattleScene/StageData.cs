using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    [System.Serializable]
    public class MonsterType
    {
        public GameObject[] monsterByDir;
    }
    public MonsterType[] monsterType;
    public GameObject[] Lmonsters;
    public Sprite sky;
    public Sprite far;
    public Sprite obj;
    public Sprite floor;
}
