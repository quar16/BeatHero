using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSet : MonoBehaviour
{
    [Header("첫번째 세팅")]
    public BeatData beatData;               //에디터에서 만든 데이터
    public NodeFunc nodeFuncPrefab;         //노드펑션의 프리팹
    public Transform nodeParent;            //노드의 부모
    public AudioSource audioSource;         //노래 연결을 위한 오디오 소스
    public AudioClip[] audioClips;          //노래들
    public GameObject center;               //플레이어와 노트의 중심
    public SummonPositionFunc[] SPF;        //센터를 기준으로 소환되는 상하좌우 포지션//아래, 오른쪽, 왼쪽, 위
    public NodeFunc[] nodeFs;               //노드를 담아두는 배열
    public Transform stageDatas;
    StageData nowStageData;
    int[] res = new int[4];
    private void Start()
    {
        BattleStart();
    }
    public void BattleStart()
    {
        beatData = Bridge.beatData;
        nowStageData = stageDatas.GetChild(Bridge.stageN).gameObject.GetComponent<StageData>();
        for (int i = 0; i < 3; i++)
        {
            skyBG[i].GetComponent<SpriteRenderer>().sprite = nowStageData.sky;
            farBG[i].GetComponent<SpriteRenderer>().sprite = nowStageData.far;
            objectBG[i].GetComponent<SpriteRenderer>().sprite = nowStageData.obj;
            floorBG[i].GetComponent<SpriteRenderer>().sprite = nowStageData.floor;
        }
        nodeFs = new NodeFunc[beatData.beatNodes.Length];
        for (int n = 0; n < beatData.beatNodes.Length; n++)
        {
            GameObject tempN = Instantiate(nodeFuncPrefab.gameObject);
            tempN.transform.SetParent(nodeParent);
            nodeFs[n] = tempN.GetComponent<NodeFunc>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SPF[i].isEmpty[j] = true;
                }
            }
            //일단 롱노트는 없는거로 생각하자

            if (!beatData.beatNodes[n].isLong)//longAdd
            {
                nodeFs[n].monsterFs = new MonsterFunc[beatData.beatNodes[n].beats.Length];
                for (int b = 0; b < beatData.beatNodes[n].beats.Length; b++)
                {
                    Beat beat = beatData.beatNodes[n].beats[b];
                    GameObject temp = Instantiate(nowStageData.monsterType[beat.Type].monsterByDir[beat.Direction]);
                    temp.transform.SetParent(SPF[beat.Direction].transform);
                    int posIndex = beat.SummonPos;
                    posIndex--;
                    if (posIndex == -1)
                        posIndex = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (SPF[beat.Direction].isEmpty[posIndex])
                        {
                            temp.transform.localPosition = SPF[beat.Direction].summonPoses[posIndex];
                            SPF[beat.Direction].isEmpty[posIndex] = false;
                            break;
                        }
                        else
                        {
                            posIndex++;
                            if (posIndex == 3)
                                posIndex = 0;
                        }
                    }
                    temp.SetActive(false);
                    nodeFs[n].monsterFs[b] = temp.GetComponent<MonsterFunc>();
                    nodeFs[n].monsterFs[b].center = center;
                    nodeFs[n].monsterFs[b].battleSet = this;
                    if (beat.Type == 1)
                        nodeFs[n].monsterFs[b].line = beat.Direction;
                    else if (beat.Type == 0)
                    {
                        nodeFs[n].monsterFs[b].line = 3 - beat.Direction;
                    }
                    else
                    {
                        nodeFs[n].monsterFs[b].line = -1;
                    }
                    //몬스터 프리팹을 만들고 데이터를 넣어줌
                }
            }
        }
        audioSource.clip = audioClips[beatData.musicID];
        StartCoroutine(StartAnime());
    }

    IEnumerator StartAnime()
    {
        yield return new WaitWhile(() => Bridge.loadOn());
        for (int i = 0; i < 60; i++)
        {
            center.transform.position += Vector3.right * 2;
            yield return null;
        }

        StartCoroutine(beltScroll());
        StartCoroutine(BattleRouting());
        StartCoroutine(CameraSet());
    }
    [Header("배경 세팅")]

    public GameObject skyBGpt;
    public GameObject farBGpt;
    public GameObject objectBGpt;
    public GameObject floorBGpt;

    public GameObject[] skyBG;
    public GameObject[] farBG;
    public GameObject[] objectBG;
    public GameObject[] floorBG;

    IEnumerator beltScroll()
    {
        while (true)
        {
            skyBGpt.transform.position += Vector3.left * 0.2f;
            farBGpt.transform.position += Vector3.left * 0.5f;
            objectBGpt.transform.position += Vector3.left * 1;
            floorBGpt.transform.position += Vector3.left * 2;
            yield return null;
        }
    }

    public AudioSource effect;
    public int nowNode = 0;
    public int nowBeat = 0;
    public int maxBeat = 0;
    IEnumerator BattleRouting()
    {
        StartCoroutine(InputRouting());
        audioSource.Play();
        maxBeat = beatData.beatNodes[0].beats.Length;
        for (int n = 0; n != beatData.beatNodes.Length; n++)
        {
            float nodeTimeS = beatData.beatNodes[n].NodeStartTime / 125.0f * (60.0f / beatData.BPM);
            float nodeTimeE = nodeTimeS + beatData.beatNodes[n].NodeEndTime / 125.0f * (60.0f / beatData.BPM);
            yield return new WaitUntil(() => audioSource.time - beatData.Offset > nodeTimeS);

            if (!beatData.beatNodes[n].isLong)
            {
                StartCoroutine(MonsterActive(n, nodeTimeE));
                effect.Play();
                for (int b = 0; b != beatData.beatNodes[n].beats.Length; b++)
                {
                    Beat beat = beatData.beatNodes[n].beats[b];
                    yield return new WaitUntil(() => audioSource.time - beatData.Offset - nodeTimeS > beat.BeatTime / 125.0f * (60.0f / beatData.BPM));
                    if (nodeFs[n].monsterFs[b] != null)
                        nodeFs[n].monsterFs[b].Summon();
                    else
                        Debug.Log("ㅁㅁㅁㅁㅁ" + n + "," + b);
                    StartCoroutine(Arrow(beat.Type, beat.Direction, nodeFs[n].monsterFs[b].transform.position, nodeTimeE - nodeTimeS - 0.25f));
                }
                yield return new WaitUntil(() => audioSource.time - beatData.Offset > nodeTimeE);

                effect.Play();
            }
            else
            {
                StartCoroutine(LongReady(nodeTimeS, nodeTimeE));
                StartCoroutine(LongCamera((nodeTimeE - nodeTimeS) * 2 - 0.5f));
            }
        }
    }
    IEnumerator LongCamera(float time)
    {
        float l = 0;
        while (l < 1)
        {
            Camera.transform.position = Vector3.Lerp(new Vector3(0, 0, -10), new Vector3(38, 0, -10), l);
            Camera.GetComponent<Camera>().orthographicSize = 54 - 18 * l;
            l += Time.deltaTime * 5;
            yield return null;
        }
        yield return new WaitForSeconds(time);

        isLongCameraAct = false;
        while (l > 0)
        {
            Camera.GetComponent<Camera>().orthographicSize = 54 - 18 * l;
            l -= Time.deltaTime * 5;
            yield return null;
        }
        Camera.GetComponent<Camera>().orthographicSize = 54;
    }

    public GameObject Camera;
    void Shake(int index)
    {
        switch (index)
        {
            case 0:
                Camera.transform.position += new Vector3(0, -3, -10);
                break;
            case 1:
                Camera.transform.position += new Vector3(5, 0, -10);
                break;
            case 2:
                Camera.transform.position += new Vector3(-3, 0, -10);
                break;
            case 3:
                Camera.transform.position += new Vector3(0, 3, -10);
                break;
        }
    }
    bool isLongCameraAct;
    IEnumerator CameraSet()
    {
        while (true)
        {
            if (!isLongCameraAct)
                Camera.transform.position = Vector3.Lerp(Camera.transform.position, Vector3.back * 10, 0.1f);
            else
                Camera.transform.position = Vector3.Lerp(new Vector3(0, 0, -10), new Vector3(38, 0, -10), 0.1f);
            yield return null;
        }
    }

    LongMonsterFunc first;
    LongMonsterFunc front;
    LongMonsterFunc now;
    IEnumerator LongReady(float start, float end)
    {
        float l = 0;
        int t = 0;
        int n = 0;
        int countN = 0;
        float gap = end - start;
        isLongCameraAct = true;
        while (l < gap)
        {
            if (t == 10)
            {
                n++;
                int randL = Random.Range(0, 3);
                GameObject temp = Instantiate(nowStageData.Lmonsters[randL]);
                effects.Play();
                now = temp.GetComponent<LongMonsterFunc>();
                now.center = center;
                now.battleSet = this;
                if (front != null)
                {
                    front.Next = now;
                }
                else
                {
                    first = now;
                }
                float angle = Random.Range(0, 2f * Mathf.PI);
                float r = Random.Range(0, 15f);
                temp.transform.position += new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
                t = -1;
                front = now;
            }
            t++;
            l += Time.deltaTime;
            yield return null;
        }
        //yield return new WaitUntil(() => audioSource.time - beatData.Offset > end);
        now = first;
        while (countN != n)
        {
            if (t == 10)
            {
                now.Active();
                countN++;
                t = -1;
                now = now.Next;
            }
            t++;
            yield return null;
        }
        nowNode++;
        maxBeat = beatData.beatNodes[nowNode].beats.Length;
    }

    string[] AttackTrigers = { "AttackDown", "AttackFront", "AttackBack", "AttackUp" };
    IEnumerator InputRouting()
    {
        while (true)
        {

            if (Input.GetKey(KeyCode.RightArrow))
            {
                isLongGuard = true;
                if (isLongCameraAct)
                {
                    animator.SetBool("isLongGuard", true);
                }
            }
            else
            {
                isLongGuard = false;
                animator.SetBool("isLongGuard", false);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
                Clicked(0);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                Clicked(1);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Clicked(2);
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Clicked(3);
            yield return null;
        }
    }
    public bool isLongGuard = false;

    public AudioSource effects;
    public Animator animator;
    public void Clicked(int index)
    {
        if (beatData.beatNodes[nowNode].isLong)
            return;
        Debug.Log("CC " + nodeFs[nowNode].monsterFs[nowBeat].l);
        int animeIndex = index;
        if (nodeFs[nowNode].monsterFs[nowBeat].l > 0)
        {
            if (nodeFs[nowNode].monsterFs[nowBeat].line == -1)
            {
                Miss();
            }
            else if (nodeFs[nowNode].monsterFs[nowBeat].line == index)
            {
                if (beatData.beatNodes[nowNode].beats[nowBeat].Type == 0)
                    animeIndex = 3 - index;
                if (nodeFs[nowNode].monsterFs[nowBeat].l < 0.4f)
                    Fast();
                else if (nodeFs[nowNode].monsterFs[nowBeat].l > 0.6f)
                    Slow();
                else
                    Perfact();
            }
            else
            {
                Debug.Log("other line " + nodeFs[nowNode].monsterFs[nowBeat].l);
            }
            Shake(animeIndex);
            animator.SetTrigger(AttackTrigers[animeIndex]);
        }
    }
    public GameObject ComboParent;
    public Text[] ComboText;
    int combo;
    IEnumerator Combo(bool isOn)
    {
        if (isOn)
        {
            if (!ComboParent.activeSelf)
                ComboParent.SetActive(true);
            combo++;
            for (int i = 0; i < 5; i++)
            {
                ComboText[i].text = combo.ToString();
            }
            for (int f = 0; f < 5; f++)
            {
                for (int i = 0; i < 5; i++)
                {
                    ComboText[i].transform.localScale = new Vector3(1 + f * 0.1f, 1 + f * 0.1f, 1);
                }
                yield return null;
            }

            for (int i = 0; i < 5; i++)
            {
                ComboText[i].transform.localScale = Vector3.one;
            }
        }
        else
        {
            combo = 0;
            for (int i = 0; i < 5; i++)
            {
                ComboText[i].text = combo.ToString();
            }
            ComboParent.SetActive(false);
        }
    }
    public Transform Life;
    float life = 5;
    public bool isCal;
    IEnumerator LifeCalc(float delta)
    {
        if (isCal)
            yield break;
        isCal = true;
        int lastLife = Mathf.CeilToInt(life);
        life = Mathf.Clamp(life + delta, 0, 4.9f);
        int nowLife = Mathf.CeilToInt(life);
        //0.1~0.9/1~1.9 ~~ 4~4.9 (0, 1~5)

        if (nowLife == 0)
        {
            Debug.Log("gameover");
        }
        float l = 0;
        if (lastLife > nowLife)
        {
            while (l < 0.1f)
            {
                Life.GetChild(lastLife - 1).transform.localScale = new Vector3(1 + l * 10, 1 + l * 10, 1);
                Life.GetChild(lastLife - 1).GetComponent<Image>().color = new Color(1, 1, 1, 1 - l * 10);
                l += Time.deltaTime;
                yield return null;
            }
            Life.GetChild(lastLife - 1).gameObject.SetActive(false);
        }
        else if (lastLife < nowLife)
        {
            Life.GetChild(nowLife - 1).transform.localScale = new Vector3(2, 2, 1);
            Life.GetChild(nowLife - 1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Life.GetChild(nowLife - 1).gameObject.SetActive(true);
            while (l < 0.1f)
            {
                Life.GetChild(nowLife - 1).transform.localScale = new Vector3(2 - l * 10, 2 - l * 10, 1);
                Life.GetChild(nowLife - 1).GetComponent<Image>().color = new Color(1, 1, 1, l * 10);
                l += Time.deltaTime;
                yield return null;
            }
            Life.GetChild(nowLife - 1).transform.localScale = new Vector3(1, 1, 1);
            Life.GetChild(nowLife - 1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        isCal = false;
    }

    public void Perfact()
    {
        Debug.Log("perfact " + nodeFs[nowNode].monsterFs[nowBeat].l);
        res[0]++;
        StartCoroutine(EffectPlay(nodeFs[nowNode].monsterFs[nowBeat].transform.position, 0));
        effects.Play();
        StartCoroutine(nodeFs[nowNode].monsterFs[nowBeat].SetHit());
        BeatCal();
        StartCoroutine(Combo(true));
        StartCoroutine(LifeCalc(0.2f));
    }
    public void Slow()
    {
        Debug.Log("Slow " + nodeFs[nowNode].monsterFs[nowBeat].l);
        res[2]++;
        StartCoroutine(EffectPlay(nodeFs[nowNode].monsterFs[nowBeat].transform.position, 1));
        effects.Play();
        StartCoroutine(nodeFs[nowNode].monsterFs[nowBeat].SetHit());
        BeatCal();
        StartCoroutine(Combo(true));
        StartCoroutine(LifeCalc(0.1f));
    }
    public void Fast()
    {
        Debug.Log("Fast " + nodeFs[nowNode].monsterFs[nowBeat].l);
        res[1]++;
        StartCoroutine(EffectPlay(nodeFs[nowNode].monsterFs[nowBeat].transform.position, 2));
        effects.Play();
        StartCoroutine(nodeFs[nowNode].monsterFs[nowBeat].SetHit());
        BeatCal();
        StartCoroutine(Combo(true));
        StartCoroutine(LifeCalc(0.1f));
    }
    public void Miss()
    {
        Debug.Log("Miss " + nodeFs[nowNode].monsterFs[nowBeat].l);
        res[3]++;
        StartCoroutine(EffectPlay(nodeFs[nowNode].monsterFs[nowBeat].transform.position, 3));
        Destroy(nodeFs[nowNode].monsterFs[nowBeat].gameObject);
        BeatCal();
        StartCoroutine(Combo(false));
        StartCoroutine(LifeCalc(-1));
    }
    public void LongPerfact(Vector3 pos)
    {
        effects.Play();
        StartCoroutine(EffectPlay(pos, 0));
        StartCoroutine(Combo(true));
        StartCoroutine(LifeCalc(0.05f));
    }
    public void LongMiss(Vector3 pos)
    {
        StartCoroutine(EffectPlay(pos, 3));
        StartCoroutine(Combo(false));
        StartCoroutine(LifeCalc(-0.1f));
    }

    public GameObject[] Effects;
    public IEnumerator EffectPlay(Vector3 pos, int index)
    {
        GameObject temp = Instantiate(Effects[index]);
        temp.transform.parent = EffParent;
        temp.transform.position = pos;
        SpriteRenderer tempS = temp.GetComponent<SpriteRenderer>();
        float r = tempS.color.r;
        float g = tempS.color.g;
        float b = tempS.color.b;
        for (float l = 0; l < 1;)
        {
            tempS.color = new Color(r, g, b, l);
            l += Time.deltaTime * 10;
            yield return null;
        }
        for (float l = 0; l < 1;)
        {
            tempS.color = new Color(r, g, b, 1 - l);
            l += Time.deltaTime * 10;
            yield return null;
        }
        Destroy(temp);
    }
    public GameObject[] DirEffects;
    public Transform EffParent;
    public IEnumerator Arrow(int type, int Dir, Vector3 pos, float time)
    {
        if (Dir == 0)
            Dir = 1;
        else if (Dir == 1)
            Dir = 2;
        else if (Dir == 2)
            Dir = 0;
        else if (Dir == 3)
            Dir = 3;

        GameObject temp_e = Instantiate(DirEffects[type]);
        GameObject temp_e2 = Instantiate(DirEffects[type]);
        temp_e.transform.parent = EffParent;
        temp_e.transform.position = pos;
        temp_e.transform.rotation = Quaternion.Euler(0, 0, 90 * Dir);
        temp_e2.transform.parent = EffParent;
        temp_e2.transform.position = pos;
        temp_e2.transform.rotation = Quaternion.Euler(0, 0, 90 * Dir);
        float delta = 0;
        for (int i = 0; i < 10; i++)
        {
            temp_e2.transform.localScale = new Vector3(1 + i * 0.1f, 1 + i * 0.1f, 1);
            temp_e2.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - i * 0.1f);
            yield return null;
            delta += Time.deltaTime;
        }
        Destroy(temp_e2);
        yield return new WaitForSeconds(time - delta);
        Destroy(temp_e);
    }

    public void BeatCal()
    {
        nowBeat++;
        if (nowBeat == maxBeat)
        {
            nowNode++;
            nowBeat = 0;
            if (nowNode == nodeFs.Length)
                StartCoroutine(GameEnd());
            else
                maxBeat = beatData.beatNodes[nowNode].beats.Length;
        }
    }

    IEnumerator MonsterActive(int n, float nodeTime)
    {
        for (int b = 0; b != beatData.beatNodes[n].beats.Length; b++)
        {
            yield return new WaitUntil(() => audioSource.time - beatData.Offset - nodeTime + 0.25f > beatData.beatNodes[n].beats[b].BeatTime / 125.0f * (60.0f / beatData.BPM));
            if (nodeFs[n].monsterFs[b] != null)
            {
                if (nodeFs[n].monsterFs[b].isActiveAndEnabled)
                    nodeFs[n].monsterFs[b].Active();
                else
                    BeatCal();
            }
            else
                Debug.Log("ㅂㅂㅂㅂㅂㅂ" + n + "," + b);
        }
    }

    IEnumerator GameEnd()
    {
        for (int i = 0; i < 10; i++)
        {
            audioSource.volume /= 2.0f;
            yield return null;
        }
        yield return new WaitForSeconds(1);

        Bridge.SceneCall(0, Scene.Result, true, false);
        yield return new WaitWhile(() => Bridge.loadOn());
        ResultFunc resultFunc = GameObject.FindGameObjectWithTag("ResultManager").GetComponent<ResultFunc>();
        resultFunc.ResultSet(res, false);
        Bridge.SceneCall(Scene.Battle, 0, false, true);
    }

    public void tempMenu()
    {
        Bridge.SceneCall(Scene.Battle, Scene.Main);
    }
}