using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLobbyFunction : MonoBehaviour
{
    public StoryLobbyFunction SLF;
    public GameObject StoryLobbyButton;
    public GameObject ChallengeLobbyButton;
    public GameObject[] Lobbies;
    //public Text ModeTitle;
    private void Update()
    {
        if (Input.GetButton("Cancel")&&!Lobbies[0].activeSelf)
        {
            ModeSelect(0);
        }
    }

    bool isSelectProcessIsGoing = false;
    public void ModeSelect(int type)
    {
        if (isSelectProcessIsGoing)
            return;
        isSelectProcessIsGoing = true;
        StartCoroutine(LobbyChange(type));
    }
    

    IEnumerator LobbyChange(int type)
    {
        //StartCoroutine(TitleChange(type));
        Lobbies[type].SetActive(true);
        float l = 0;
        if (type != 0)
        {
            while (l != 30)
            {
                StoryLobbyButton.transform.localPosition -= new Vector3(33, 0, 0);
                ChallengeLobbyButton.transform.localPosition += new Vector3(33, 0, 0);
                l++;
                yield return null;
            }
            Lobbies[0].SetActive(false);
        }
        else
        {
            while (l != 30)
            {
                StoryLobbyButton.transform.localPosition += new Vector3(33, 0, 0);
                ChallengeLobbyButton.transform.localPosition -= new Vector3(33, 0, 0);
                l++;
                yield return null;
            }
            Lobbies[1].SetActive(false);
            Lobbies[2].SetActive(false);
        }
        isSelectProcessIsGoing = false;
    }

    //string[] TitleTexts = { "Mode Select", "Story Mode", "Challenge" };
    //IEnumerator TitleChange(int type)
    //{
    //    int temp = ModeTitle.text.Length;
    //    for (int i = temp; i != -1; i--)
    //    {
    //        ModeTitle.text = ModeTitle.text.Substring(0, i);
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(0.2f);
    //    temp = TitleTexts[type].Length;
    //    for (int i = 0; i < temp; i++)
    //    {
    //        ModeTitle.text = TitleTexts[type].Substring(0, i + 1);
    //        yield return null;
    //    }
    //}

    public Sprite[] Difficulties;
    public Image DifficultyIcon;
    int DifficultyIndex = 0;
    public void DifficultySetting()
    {
        DifficultyIndex++;
        if (DifficultyIndex == 3)
            DifficultyIndex = 0;
        DifficultyIcon.sprite = Difficulties[DifficultyIndex];
    }

    GameObject OptionPage;
    public void OptionOpen()
    {
        if(OptionPage==null)
        OptionPage= GameObject.FindGameObjectWithTag("OptionPage").transform.GetChild(0).gameObject;
        OptionPage.SetActive(true);
    }//this maybe in Bridge after

    public GameObject CharacterSelectPage;
    public void CharacterSelectPageOpen()
    {
        CharacterSelectPage.SetActive(true);
    }
    public void CharacterSelectPageClose()
    {
        CharacterSelectPage.SetActive(false);
    }

    public void StageStart(int selectedNumber)
    {
        //Bridge.beatData = stages[SLF.HighlightedStageNumber].beatDatas[selectedNumber];
        if (selectedNumber > 1)
            selectedNumber = 1;
        Bridge.SetBeatData(SLF.HighlightedStageNumber, selectedNumber, DifficultyIndex);
        Bridge.SceneCall(Scene.Main, Scene.Battle);
    }

}
