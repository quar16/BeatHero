using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

static class BXC
{
    public static float BX;
    public static bool isPlaying;
}

public class EditorSceneManager : MonoBehaviour
{
    #region 시작 세팅
    public EditorSceneSetting ESS;

    public AudioSource Music;
    AudioClip MusicClip;
    int BPM;
    public float Offset;
    public BeatData beatData;
    public Text MusicT;
    public Text BPMT;
    public Text FileT;
    public Text OffsetT;

    public NoteMakerCalling Score;
    NoteMakerCalling[] ScorePrefabs;
    public GameObject ScoreParent;
    public float r;

    public NowPositionArea NPA;

    private void Start()
    {
        if (MusicClip == null)
        {
            if (ESS.Music != null)
            {
                MusicClip = ESS.Music;
                Music.clip = MusicClip;
                BPM = ESS.BPM;
                Offset = ESS.Offset;
                beatData = ESS.beatData;
                MusicT.text = "노래 | " + Music.clip.name;
                BPMT.text = "BPM | " + BPM.ToString();
                OffsetT.text = "오프셋 | " + Offset.ToString();
                FileT.text = "파일명 | " + beatData.gameObject.name;
            }
            else
                Debug.Log("Music is not exist. Please set a music on ※Setting");
        }
        Music.Play();
        Music.Pause();
        r = ((BPM / 4.0f) * (Music.clip.length / 60.0f));
        int _r = (int)(r / 1);
        Score.ESM = this;
        ScorePrefabs = new NoteMakerCalling[_r];
        for (int i = 0; i < _r; i++)
        {
            GameObject prefab = Instantiate(Score.gameObject);
            prefab.transform.SetParent(ScoreParent.transform);
            prefab.transform.localPosition = new Vector3(500 * i, 0, 0);
            ScorePrefabs[i] = prefab.GetComponent<NoteMakerCalling>();
        }
        NPA.transform.localScale = new Vector3(1650 * (1920.0f / (500.0f * _r)), 1, 1);
        NPA.scoreNumber = r;
        NPA.scale = NPA.transform.localScale.x / 2.0f;
        NPA.start = -825 + NPA.scale;
        NPA.end = 825 - NPA.scale;
        NPA.gauge = NPA.end - NPA.start;

        Load();
    }

    void Load()
    {
        if (beatData.beatNodes == null)
            return;
        for (int n = 0; n < beatData.beatNodes.Length; n++)
        {
            if (!beatData.beatNodes[n].isLong)
            {
                GameObject tempN = Instantiate(StartNodePrefab);
                tempN.GetComponent<StartNodeFunc>().ESM = this;
                tempN.transform.SetParent(NodeParent);
                tempN.transform.localPosition = new Vector3(beatData.beatNodes[n].NodeStartTime, 0, 0);
                tempN.GetComponent<StartNodeFunc>().EndNode.transform.localPosition = new Vector3(beatData.beatNodes[n].NodeEndTime, 0, 0);
                tempN.GetComponent<StartNodeFunc>().AreaSet();
                for (int b = 0; b < beatData.beatNodes[n].beats.Length; b++)
                {
                    Beat tempB = beatData.beatNodes[n].beats[b];
                    GameObject temp = Instantiate(Notes[tempB.Type]);
                    temp.transform.SetParent(tempN.transform);
                    float y = tempB.Direction * 150 - 225;
                    temp.transform.localPosition = new Vector3(tempB.BeatTime, y, tempB.Type);

                    GameObject temp2 = Instantiate(Notes[tempB.Type]);
                    temp2.transform.SetParent(tempN.transform.GetChild(2));
                    temp2.transform.localPosition = temp.transform.localPosition;
                    temp.GetComponent<BeatPrefabFunc>().copy = temp2;
                    temp.GetComponent<BeatPrefabFunc>().summonPos = tempB.SummonPos;
                }
            }
            else
            {
                GameObject tempN = Instantiate(LongNotePrefab);
                tempN.transform.GetChild(1).GetComponent<LongNotePosHandle>().ESM = this;
                tempN.transform.SetParent(NodeParent);
                tempN.transform.localPosition = new Vector3(beatData.beatNodes[n].NodeStartTime, 0, 1);
                tempN.transform.GetChild(1).GetComponent<LongNotePosHandle>().SizeHandle.transform.localPosition = new Vector3(beatData.beatNodes[n].NodeEndTime, 0, 1);
                tempN.transform.GetChild(1).GetComponent<LongNotePosHandle>().AreaSet();
            }
        }
    }
    #endregion

    #region 노래 정지, 이동 UI
    public void MusicToggle()
    {
        if (!Music.isPlaying)
        {
            BXC.isPlaying = true;
            Music.Play();
        }
        else
        {
            BXC.isPlaying = false;
            Music.Pause();
        }
    }//노래 정지, 재생
    public void MusicDialMove()
    {
        float d = (Input.mousePosition.x - 55) / 1650;
        if (d < 0)
            d = 0;
        if (d >= 1)
            d = 0.999f;
        Music.time = d * Music.clip.length;
    }//노래 시간 위치 이동
    #endregion

    #region 되감기
    public GameObject RepeatDialLeft;
    public GameObject RepeatDialRight;
    bool repeatOn = false;
    public float repeatStartD;
    public float repeatEndD;
    public void RepeatToggle()
    {
        if (repeatOn)
        {
            RepeatDialLeft.SetActive(false);
            RepeatDialRight.SetActive(false);
            repeatOn = false;
        }
        else
        {
            RepeatDialLeft.SetActive(true);
            RepeatDialRight.SetActive(true);
            repeatOn = true;
        }
    }//되감기 다이얼 끄기/켜기
    #endregion

    #region 노래 재생update
    public GameObject MusicDial;
    public GameObject BeatLine;
    private void Update()
    {
        float Time = Mathf.Clamp(Music.time - Offset, 0, float.MaxValue);
        float d = Time / Music.clip.length;
        if (d > 0.999f)
        {
            BXC.isPlaying = false;
            Music.Pause();
            Music.time = Music.clip.length *0.99999f;
        }
        MusicDial.transform.localPosition = new Vector3(-825 + 1650 * d, 0, 0);
        BeatLine.transform.localPosition = new Vector3(125.0f * Time * BPM / 60.0f, 0, 0);
        if (repeatOn)
        {
            float repeatStart = repeatStartD * Music.clip.length;
            float repeatEnd = repeatEndD * Music.clip.length;
            if (Music.time < repeatStart || Music.time > repeatEnd)
            {
                Music.time = repeatStart;
            }
        }
        BXC.BX = BeatLine.transform.position.x;
    }
    #endregion

    public int NoteType;
    public Transform NodeParent;
    public GameObject StartNodePrefab;
    public GameObject LongNotePrefab;
    public void StartNodeMake()//스타트 노드 만들기&롱노트 만들기
    {
        if (NoteType == 0)
        {
            GameObject temp = Instantiate(StartNodePrefab);
            temp.GetComponent<StartNodeFunc>().ESM = this;
            temp.transform.SetParent(NodeParent);
            temp.transform.position = Input.mousePosition;
            temp.transform.localPosition = new Vector3(temp.transform.localPosition.x, 0, 0);
            if (terms != 0)
            {
                float x = temp.transform.localPosition.x;
                float index = 250.0f / Mathf.Pow(2, terms);
                int a = Mathf.RoundToInt(x / index);
                temp.transform.localPosition = new Vector3(a * index, 0, 0);
            }
        }
        else if (NoteType == 4)
        {
            GameObject temp = Instantiate(LongNotePrefab);
            temp.transform.GetChild(1).GetComponent<LongNotePosHandle>().ESM = this;
            temp.transform.SetParent(NodeParent);
            temp.transform.position = Input.mousePosition;
            temp.transform.localPosition = new Vector3(temp.transform.localPosition.x, 0, 1);
            if (terms != 0)
            {
                float x = temp.transform.localPosition.x;
                float index = 250.0f / Mathf.Pow(2, terms);
                int a = Mathf.RoundToInt(x / index);
                temp.transform.localPosition = new Vector3(a * index, 0, 1);
            }
        }
    }

    public GameObject[] Notes;
    public void NoteMake(Transform parent)//스타트 노드 위에 비트 만들기
    {
        if (NoteType > 0 && NoteType < 4)
        {
            GameObject temp = Instantiate(Notes[NoteType - 1]);
            temp.transform.SetParent(parent);
            float z = temp.transform.localPosition.z;
            temp.transform.position = Input.mousePosition;
            float y = temp.transform.localPosition.y + 225;
            y = Mathf.RoundToInt(y / 150.0f) * 150;
            y -= 225;
            temp.transform.localPosition = new Vector3(temp.transform.localPosition.x, y, z);
            if (terms != 0)
            {
                float x = temp.transform.localPosition.x;
                float index = 250.0f / Mathf.Pow(2, terms);
                int a = Mathf.RoundToInt(x / index);
                temp.transform.localPosition = new Vector3(a * index, y, z);
            }
            GameObject temp2 = Instantiate(Notes[NoteType - 1]);
            temp2.transform.SetParent(parent.GetChild(2));
            temp2.transform.localPosition = temp.transform.localPosition;
            temp.GetComponent<BeatPrefabFunc>().copy = temp2;
        }
    }

    public GameObject NoteSelectHighlight;
    public void NoteTypeChange(int i)//노트 타입 변경
    {
        NoteType = i;
        NoteSelectHighlight.transform.localPosition = new Vector3(200 * i, 0, 0);
    }

    public int terms = 1;
    public void BeatTermToggle()//비트 간격 변경
    {
        for(int i = 0; i < ScorePrefabs.Length; i++)
        {
            ScorePrefabs[i].TermChange(terms);
        }
        terms = (terms + 1) % 4;
    }

    bool infoOff = true;
    public GameObject Info;
    public void Infomation()
    {
        if (infoOff)
            Info.transform.localPosition = new Vector3(660, -275, 0);
        else
            Info.transform.localPosition = new Vector3(1260, -275, 0);
        infoOff = !infoOff;
    }

    public void Save()
    {
        int NodeNumber = NodeParent.childCount;
        beatData.BPM = BPM;
        beatData.Offset = Offset;
        beatData.beatNodes = new BeatNode[NodeNumber];
        BeatData tempData = new BeatData();
        tempData.beatNodes = new BeatNode[NodeNumber];
        for(int n = 0; n < NodeNumber; n++)
        {
            Transform Node = NodeParent.GetChild(n);
            tempData.beatNodes[n].NodeStartTime = Node.transform.localPosition.x;
            tempData.beatNodes[n].NodeEndTime = Node.GetChild(2).transform.localPosition.x;
            if (Node.transform.position.z == 0)
            {
                tempData.beatNodes[n].isLong = false;
                tempData.beatNodes[n].beats = new Beat[Node.childCount - 3];
                for (int b = 0; b < Node.childCount - 3; b++)
                {
                    tempData.beatNodes[n].beats[b].BeatTime = Node.GetChild(3 + b).transform.localPosition.x;
                    tempData.beatNodes[n].beats[b].Type = (int)Node.GetChild(3 + b).transform.localPosition.z;
                    tempData.beatNodes[n].beats[b].Direction = (int)((Node.GetChild(3 + b).transform.localPosition.y + 225) / 150);
                    tempData.beatNodes[n].beats[b].SummonPos = Node.GetChild(3 + b).GetComponent<BeatPrefabFunc>().summonPos;
                }
            }
            else
            {
                tempData.beatNodes[n].NodeStartTime = Node.transform.localPosition.x;
                tempData.beatNodes[n].NodeEndTime = Node.GetChild(2).transform.localPosition.x;
                tempData.beatNodes[n].isLong = true;
            }
        }
        int minIndex = 0;
        for (int n = 0; n < NodeNumber; n++)
        {
            minIndex = n;
            for(int t = n; t < NodeNumber; t++)
            {
                if(tempData.beatNodes[minIndex].NodeStartTime > tempData.beatNodes[t].NodeStartTime)
                {
                    minIndex = t;
                }
            }
            BeatNode tempN = tempData.beatNodes[n];
            tempData.beatNodes[n] = tempData.beatNodes[minIndex];
            tempData.beatNodes[minIndex] = tempN;
            for (int b = 0; b < tempData.beatNodes[n].beats.Length; b++)
            {
                minIndex = b;
                for (int t = b; t < tempData.beatNodes[n].beats.Length; t++)
                {
                    if (tempData.beatNodes[n].beats[minIndex].BeatTime > tempData.beatNodes[n].beats[t].BeatTime)
                    {
                        minIndex = t;
                    }
                }
                Beat tempB = tempData.beatNodes[n].beats[b];
                tempData.beatNodes[n].beats[b] = tempData.beatNodes[n].beats[minIndex];
                tempData.beatNodes[n].beats[minIndex] = tempB;
            }
        }
        beatData.beatNodes = tempData.beatNodes;
    }
    
    public void Tracking()
    {
        NPA.isTracking = !NPA.isTracking;
    }
    public GameObject LoadBG;
    public void TestPlay()
    {
        LoadBG.SetActive(true);
        StartCoroutine(SceneProcess());
    }


    IEnumerator SceneProcess()
    {
        AsyncOperation asyncLoad;

        asyncLoad = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        asyncLoad = SceneManager.UnloadSceneAsync("EditorScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}
