using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RogueGameManager : MonoBehaviour
{
    public static RogueGameManager Instance;
    GameObject mainMenu;
    GameObject endGameScreen;
    GameObject canvas;
    private void Awake()
    {
        Instance = this;
        canvas = GameObject.Find("Canvas");
        mainMenu = canvas.transform.Find("StartMenu").gameObject;
        endGameScreen = canvas.transform.Find("EndScreenContainer").gameObject;
    }

    void Start()
    {
        PlayerUIManager.Instance.DisablePlayerUi();
        if (!mainMenu.activeInHierarchy) {
            BoardManager.Instance.ResetCounters();

            BoardManager.Instance.CreatePlayer();
            BoardManager.Instance.StartNewLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowEndScreen(int roomsCleared,int enemiesKilled,int turns) {
        endGameScreen.SetActive(true);
        var infoContainer = endGameScreen.transform.Find("InfoContainer");

        var enemiesCounter = infoContainer.Find("Enemies-killed-counter").gameObject;
        SetText(enemiesCounter, enemiesKilled);

        var turnsCounter = infoContainer.Find("Turns-spent-counter").gameObject;
        SetText(turnsCounter, turns);

        var roomsCounter = infoContainer.Find("Rooms-cleared-counter").gameObject;
        SetText(roomsCounter, roomsCleared);

    }

    private void SetText(GameObject textGameObject, int text) {
        textGameObject.GetComponent<Text>().text = text.ToString();
    }


    public void StartGameClicked() {
        mainMenu.SetActive(false);
        BoardManager.Instance.ResetCounters();

        BoardManager.Instance.CreatePlayer();
        BoardManager.Instance.StartNewLevel();
    }

    public void RestartGameClicked()
    {
        mainMenu.SetActive(true);
        endGameScreen.SetActive(false);
    }
}
