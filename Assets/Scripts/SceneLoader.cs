using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//씬을 다루는 스크립트, 이전에 사용하던대로, 딱히 손볼 것 없음
static class Bridge
{
    static SceneLoader sceneLoader;
    public static BeatData beatData;
    public static int StageType;
    public static int stageN;
    public static int level;
    public static int difficulty;
    static SceneLoader SceneLoaderSet()
    {
        if (sceneLoader == null)
            sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        return sceneLoader;
    }

    public static void SetBeatData(int _stageN, int _level, int _difficulty)
    {
        stageN = _stageN;
        level = _level;
        difficulty = _difficulty;
        beatData = SceneLoaderSet().stages[stageN].levels[level].beatDatas[difficulty];
    }

    public static void SetNextBeatData(int _stageN, int _level, int _difficulty)
    {
        if (SceneLoaderSet().stages[stageN].levels[level].beatDatas.Length == _level + 1)
        {
            stageN++;
            level = 0;
        }
        beatData = SceneLoaderSet().stages[stageN].levels[level].beatDatas[difficulty];
    }

    public static void SceneCall(Scene CloseScene, Scene OpenScene, bool closeEfct = true, bool openEfct = true)
    {
        SceneLoaderSet().SceneLoadCall(CloseScene, OpenScene, closeEfct, openEfct);
    }

    public static bool loadOn()
    {
        return SceneLoaderSet().GetComponent<SceneLoader>().LoadSceneIsGoing;
    }
}

public enum Scene { Title = 1, Main, Battle, Result, CharacterSelect, Option, Editor }

public class SceneLoader : MonoBehaviour
{
    public GameObject TouchPreventArea;
    public Image LoadImage;
    public Image BattleSceneLoadImage;
    public Text MusicTitle;
    public Text Composer;
    public Image[] MusicCover;
    public void Awake()
    {
        Screen.SetResolution(1920, 1080, true);//이건 일단 남겨두자, 스크린 사이즈 변화에 대응해야됨
        StartCoroutine(FirstStart());
    }
    IEnumerator FirstStart()
    {
        yield return StartCoroutine(LoadScene(0, Scene.Option, false, false));
        yield return StartCoroutine(LoadScene(0, Scene.Title, false, true));
    }

    public void SceneLoadCall(Scene CloseScene, Scene OpenScene, bool closeEfct, bool openEfct)//씬을 변경하는 함수
    {
        StartCoroutine(LoadScene(CloseScene, OpenScene, closeEfct, openEfct));
    }

    public bool LoadSceneIsGoing = false;

    IEnumerator LoadScene(Scene CloseScene, Scene OpenScene, bool closeEfct, bool openEfct)
    {
        if (LoadSceneIsGoing)
            yield break;
        LoadSceneIsGoing = true;
        TouchPreventArea.SetActive(true);
        if (closeEfct)
        {
            float l = 0;
            while (l < 1)
            {
                l += Time.deltaTime;

                if (OpenScene == Scene.Battle)
                    BattleSceneLoadImageAlpha(l);
                else
                    LoadImageAlpha(l);

                yield return null;
            }
        }
        if (CloseScene != 0)
        {
            yield return StartCoroutine(SceneProcess(false, CloseScene));
        }
        loadTime = 0;
        if (OpenScene != 0)
        {
            yield return StartCoroutine(SceneProcess(true, OpenScene));
            if (OpenScene == Scene.Battle)
                yield return new WaitForSeconds(2 - loadTime);
        }
        if (openEfct)
        {
            float l = 1;
            while (l > 0)
            {
                l -= Time.deltaTime;

                if (OpenScene == Scene.Battle)
                    BattleSceneLoadImageAlpha(l);
                else
                    LoadImageAlpha(l);

                yield return null;
            }
        }
        LoadSceneIsGoing = false;
        TouchPreventArea.SetActive(false);
    }
    public float loadTime;
    IEnumerator SceneProcess(bool open, Scene scene)
    {
        string sceneName = scene.ToString() + "Scene";

        AsyncOperation asyncLoad;

        if (open)
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        else
            asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            loadTime += Time.deltaTime;
            //Debug.Log("Priority = " + asyncLoad.priority + ", Progress = " + asyncLoad.progress);
            yield return null;
        }
        Debug.Log(loadTime);

    }

    public void LoadImageAlpha(float l)
    {
        LoadImage.color = new Color(0, 0, 0, l);
    }

    public void BattleSceneLoadImageAlpha(float l)
    {
        BattleSceneLoadImage.color = new Color(1, 1, 1, l);
        MusicTitle.color = new Color(0, 0, 0, l);
        Composer.color = new Color(0, 0, 0, l);
    }


    [System.Serializable]
    public struct Stages
    {
        public Levels[] levels;
    }
    [System.Serializable]
    public struct Levels
    {
        public BeatData[] beatDatas;
    }

    public Stages[] stages;
}

