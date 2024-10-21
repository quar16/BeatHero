using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLobbyFunction : MonoBehaviour
{
    public GameObject[] SongIcons;
    public GameObject[] Stages;
    public int[] StageNumberBySongs;
    public int SongCount = 1;
    public int LastPlayedStageNumber;
    public int HighlightedStageNumber;

    private void OnEnable()
    {
        HighlightedStageNumber = LastPlayedStageNumber;
        for(int i = 0; i < SongCount; i++)
        {
            SongIcons[i].transform.localScale = new Vector3(1, 1, 1);
            if (i < HighlightedStageNumber - 1)
            {
                SongIcons[i].transform.localPosition = new Vector3(-1150, -250, 0);
            }
            if (i == HighlightedStageNumber - 1)
                SongIcons[i].transform.localPosition = new Vector3(-450, 0, 0);
            if (i == HighlightedStageNumber)
            {
                SongIcons[i].transform.localPosition = new Vector3(0, 100, 0);
                SongIcons[i].transform.localScale = new Vector3(1.25f, 1.25f, 1);
            }
            if(i == HighlightedStageNumber + 1)
            {
                SongIcons[i].transform.localPosition = new Vector3(450, 0, 0);
            }
            if (i > HighlightedStageNumber + 1)
            {
                SongIcons[i].transform.localPosition = new Vector3(1150, -250, 0);
            }
        }
        ShowStages();
    }

    public bool isMoving = false;
    public void ShowStages()
    {
        for (int i = 0; i < 5; i++)
            Stages[i].SetActive(false);

        int count = StageNumberBySongs[HighlightedStageNumber];
        Stages[4].SetActive(true);
        Stages[4].transform.localPosition = new Vector3(80 * (count - 1), -270, 0);
        for (int i = 0; i < count - 1; i++)
        {
            Stages[i].SetActive(true);
            Stages[i].transform.localPosition = new Vector3(160 * i - 80 * (count - 1), -270, 0);
        }
    }
    public void LeftMoveStart()//L(-1) or R(+1)
    {
        if (HighlightedStageNumber == SongCount - 1)
            return;
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(LeftMoving());
        }
    }
    public void RightMoveStart()//L(-1) or R(+1)
    {
        if (HighlightedStageNumber == 0)
            return;
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(RightMoving());
        }
    }
    IEnumerator LeftMoving()
    {
        int HSN = HighlightedStageNumber;
        float f = 20;
        for (int i = 0; i != f + 1; i++)
        {
            SongIcons[HSN].transform.localPosition = new Vector3(-450 * (i / f), 100 * (1 - i / f), 0);
            SongIcons[HSN].transform.localScale = new Vector3(1.25f - 0.25f * (i / f), 1.25f - 0.25f * (i / f), 1);

            SongIcons[HSN + 1].transform.localPosition = new Vector3(450 * (1 - i / f), 100 * (i / f), 0);
            SongIcons[HSN + 1].transform.localScale = new Vector3(1 + 0.25f * (i / f), 1 + 0.25f * (i / f), 1);

            if (HSN != 0)
                SongIcons[HSN - 1].transform.localPosition = new Vector3((-450 - 700 * (i / f)), -250 * (i / f), 0);

            if (HSN != SongCount - 2)
                SongIcons[HSN + 2].transform.localPosition = new Vector3(1150 - (700 * (i / f)), 250 * (i / f - 1), 0);

            yield return null;
        }//d = -1 // 1150 -> 450 // -250 -> 0
        HighlightedStageNumber++;
        ShowStages();
        isMoving = false;
    }
    IEnumerator RightMoving()
    {
        int HSN = HighlightedStageNumber;
        float f = 20;
        for (int i = 0; i != f + 1; i++)
        {
            SongIcons[HSN].transform.localPosition = new Vector3(450 * (i / f), 100 * (1 - i / f), 0);
            SongIcons[HSN].transform.localScale = new Vector3(1.25f - 0.25f * (i / f), 1.25f - 0.25f * (i / f), 1);

            SongIcons[HSN - 1].transform.localPosition = new Vector3(-450 * (1 - i / f), 100 * (i / f), 0);
            SongIcons[HSN - 1].transform.localScale = new Vector3(1 + 0.25f * (i / f), 1 + 0.25f * (i / f), 1);

            if (HSN != SongCount - 1)
                SongIcons[HSN + 1].transform.localPosition = new Vector3((450 + 700 * (i / f)), -250 * (i / f), 0);

            if (HSN != 1)
                SongIcons[HSN - 2].transform.localPosition = new Vector3(-1150 + (700 * (i / f)), 250 * (i / f - 1), 0);

            yield return null;
        }//d = -1 // 1150 -> 450 // -250 -> 0
        HighlightedStageNumber--;
        ShowStages();
        isMoving = false;
    }
}




/*
        int a = target.transform.childCount;
        StageCount = a;
        Icons = new GameObject[a];
        for (int i = 0; i < a; i++)
        {
            Icons[i] = target.transform.GetChild(i).gameObject;
        }
 */
