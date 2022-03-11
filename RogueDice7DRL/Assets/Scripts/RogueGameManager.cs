using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RogueGameManager : MonoBehaviour
{

    GameObject mainMenu;
    BoardManager boardManager;

    private void Awake()
    {
        mainMenu = GameObject.Find("StartMenu");
        GameObject startGameButton = GameObject.Find("StartGameButton");
        if (startGameButton!= null)
        {
            startGameButton.GetComponent<Button>().onClick.AddListener(StartGameClicked);
        }
        boardManager = GetComponent<BoardManager>();
    }

    void Start()
    {
        PlayerUIManager.Instance.DisablePlayerUi();
        GameObject startGameButton = GameObject.Find("StartGameButton");
        if (startGameButton == null)
        {
            boardManager.StartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartGameClicked() {
        mainMenu.SetActive(false);
        boardManager.StartGame();
    }
}
