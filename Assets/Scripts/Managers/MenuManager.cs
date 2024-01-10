using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Player Creation")]
    [SerializeField] private GameObject playerCreationMenu;
    [SerializeField] private TMP_InputField playerNameText;

    [Header("In Game Menu")]
    [SerializeField] private GameObject inGameMenu;

    private void Start()
    {
        Application.targetFrameRate = 60;

        GameManager.instance.SetState(GameState.Menu);

        GameManager.instance.ReadPlayerData();

        if (GameManager.instance.playerData == null)
            SetPlayerCreationMenu(true);
        else
            GridManager.instance.NextLevel();
    }

    public void SetPlayerCreationMenu(bool choice)
    {
        playerCreationMenu.SetActive(choice);
    }

    public void SetInGameMenu(bool choice)
    {
        inGameMenu.SetActive(choice);
    }

    public void SetPlayer()
    {
        if (playerNameText.text == "")
            return;

        PlayerData newPlayerData = new PlayerData()
        {
            level = "1",
            name = playerNameText.text
        };


        GameManager.instance.SavePlayerData(newPlayerData);

        GameManager.instance.SetNewPlayer(newPlayerData);

        SetPlayerCreationMenu(false);

        GridManager.instance.NextLevel();
    }

    #region Level Phase

    [Header("Level Phase")]
    [SerializeField] private Animation levelPhaseAnimation;
    [SerializeField] private TextMeshProUGUI levelPhaseText;

    public async void StartLevelPhaseAnimation(bool result)
    {
        if (result)
            levelPhaseText.text = "Next Level!";
        else
            levelPhaseText.text = "Try Again!";

        levelPhaseAnimation.gameObject.SetActive(true);
        levelPhaseAnimation.Play();

        await UniTask.WaitUntil(() => !levelPhaseAnimation.isPlaying);
        
        levelPhaseAnimation.gameObject.SetActive(false);
    }



    #endregion


    public TextMeshProUGUI errorText;
    public void AddError(string error) => errorText.text += error+"\n";

}
