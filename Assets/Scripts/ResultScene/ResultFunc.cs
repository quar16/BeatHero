using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultFunc : MonoBehaviour
{
    int stageN = 0;
    int level = 0;
    int difficulty = 0;
    public Text[] Res_Text;//p,f,l,m
    public Button NextButton;
    public void ResultSet(int[] res, bool isSuccess)
    {
        stageN = Bridge.stageN;
        level = Bridge.level;
        difficulty = Bridge.difficulty;
        if (!isSuccess)
        {
            NextButton.interactable = false;
        }
        StartCoroutine(ResultSetting(res));
    }
    IEnumerator ResultSetting(int[] res)
    {
        yield return new WaitWhile(() => Bridge.loadOn());
        for (int i = 0; i < 4; i++)
        {
            for (int r = 0; r < res[i]; r++)
            {
                Res_Text[i].text = r.ToString();
                yield return null;
            }
            yield return null;
        }
    }


    public void Replay()
    {
        Bridge.SetBeatData(stageN, level, difficulty);
        Bridge.SceneCall(Scene.Result, Scene.Battle);
    }

    public void MainMenu()
    {
        Bridge.SceneCall(Scene.Result, Scene.Main);
    }

    public void StageSelect()
    {
        Bridge.SceneCall(Scene.Result, Scene.Main);
    }

    public void Next()
    {
        Bridge.SetNextBeatData(stageN, level, difficulty);
        Bridge.SceneCall(Scene.Result, Scene.Battle);
    }
}
