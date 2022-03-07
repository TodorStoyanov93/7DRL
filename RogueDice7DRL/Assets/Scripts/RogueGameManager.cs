using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RogueGameManager : MonoBehaviour
{

    GameObject mainMenu;
    BoardManager boardManager;
    // Start is called before the first frame update

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
