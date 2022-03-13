using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    private int roomsCleared = 0;
    private int enemiesKilled = 0;
    public int playerTurns = 0;

    public List<Texture2D> levelTextures;
    public List<TileScriptableObject> levelMapping;

    public TileScriptableObject invalidTile;

    private GameObject boardGameObject;

    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;

    public Unit playerUnit;

    public List<Unit> enemyUnits;

    public int levelReached = 1;

    public Room room;

    public GameObject healthBarPrefab;

    public static BoardManager Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        this.boardGameObject = GameObject.Find("Board");
        int seed = 5;
        //Random.InitState(seed);
        

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayer() {
        var player = CreatePlayerGameObject();
        var healthBar = Instantiate(healthBarPrefab,player.transform);
        playerUnit = new PlayerControlledUnit()
        {
            maxHealth = 10,
        };
        playerUnit.SetHealth(10);
        
        playerUnit.SetGameobject(player);
        for (var i = 0; i < 2; i++)
        {
            AddSingleRandomDiceToPlayer();
        }
    }

    private void AddSingleRandomDiceToPlayer() {
        DiceData randomDice = DiceManager.Instance.GetRandomDiceFromList();

        ActivatableDice activatableDice = ActivatableDice.CreateActivatableDice(true, randomDice);
        playerUnit.dices.Add(activatableDice);

    }



    public void StartNewLevel() {
        enemyUnits = new List<Unit>();
        var room = GenerateRoom();
        this.room = room;

        DrawRoom(room);


        var startPos = GetPlayerStartingPosition(room);
        playerUnit.gameObject.transform.position = new Vector3(startPos.x,startPos.y,0);
        TurnSystem.Instance.units.Insert(0,playerUnit);

        for (var i = 0; i < levelReached; i++) { 
            var randomWalkablePos = GetRandomWalkablePosition(room);
            var enemy = CreateEnemyGameObject(randomWalkablePos);
            var healthBar = Instantiate(healthBarPrefab, enemy.transform);
            Unit enemyUnit = new AIControlledUnit()
            {
                maxHealth = 5,
            };
            enemyUnit.SetHealth(5);
            enemyUnit.SetGameobject(enemy);
            TurnSystem.Instance.units.Add(enemyUnit);
            enemyUnits.Add(enemyUnit);
        }

        CameraController.Instance.SnapTo(playerUnit.gameObject);

        PlayerUIManager.Instance.ClearCardView();
        PlayerUIManager.Instance.DrawCardView();


        PlayerUIManager.Instance.EnablePlayerUi();

        InputController.Instance.WaitForInput();

        TurnSystem.Instance.BeginFirstTurn();
    }

    internal void MakeUnitDie(Unit unit)    
    {
        if (enemyUnits.Contains(unit)) {
            Destroy(unit.gameObject);
            enemyUnits.Remove(unit);
            TurnSystem.Instance.RemoveUnit(unit);
            if (enemyUnits.Count == 0 ) {
                EndLevel();
            }
            enemiesKilled++;
        }
        if (playerUnit == unit) {
            Destroy(unit.gameObject);
            playerUnit = null;
            ResetBoardManager();
            
            EndGameScreen(false);
        }

        if (unit == TurnSystem.Instance.currentUnit)
        {
            TurnSystem.Instance.EndTurn();
        }
    }

    public void ResetBoardManager() {
        ClearRoom(room);
        room = null;
        foreach (var enemy in enemyUnits)
        {
            Destroy(enemy.gameObject);
        }
        enemyUnits.Clear();

        TurnSystem.Instance.ResetTurnSystem();

        OverlayController.Instance.ClearHighlightForCursor();
        OverlayController.Instance.ClearTargetTiles();
        OverlayController.Instance.ClearWalkableTiles();


        InputHandler.Instance.ResetInputHandler();
        PlayerUIManager.Instance.DisablePlayerUi();
        InputController.Instance.StopWaitingForInput();

    }

    void EndLevel() {
        levelReached++;
        roomsCleared++;
        ResetBoardManager();
        if (levelReached < 5) {
            AddSingleRandomDiceToPlayer();
            StartNewLevel();
        } else {
            EndGameScreen(true);
        }
    }

    void EndGameScreen(bool won)
    {
        RogueGameManager.Instance.ShowEndScreen(roomsCleared,enemiesKilled,playerTurns,won);
    }

    public void ResetCounters() {
        roomsCleared = 0;
        playerTurns = 0;
        enemiesKilled = 0;
    }



    private static Vector2Int GetPlayerStartingPosition(Room room) {
        var width = room.GetWidth();
        var height = room.GetHeight();
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++)
            {
                var vector2Pos = new Vector2Int(i,j);
                if (room.GetTile(vector2Pos).so.isWalkable) {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int();
    }

    private static Vector2Int GetRandomWalkablePosition(Room room)
    {
        var width = room.GetWidth();
        var height = room.GetHeight();

        var tries = 10;
        while (tries > 0) { 
            var tilePosToCheck = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            if (room.GetTile(tilePosToCheck).so.isWalkable) {
                return tilePosToCheck;
            }
            tries--;
        }
        return new Vector2Int();
    }

    private Room GenerateRoom() {

        var imageIndex = Random.Range(0, levelTextures.Count);
        Debug.Log("imageIndex" + imageIndex);
        var image = levelTextures[imageIndex];
        Room room = GenerateRoomFromTexture2D(image);
        return room;
    }

    private Room GenerateRoomFromTexture2D(Texture2D source)
    {
        Room room = new Room();
        int imageWidth = source.width;
        int imageHeight = source.height;

        //int width = Mathf.Min(imageWidth, 60);
        //int height = Mathf.Min(imageHeight, 25);
        Color[] pixels = source.GetPixels();

        var tiles = new TileDescriptor[imageHeight, imageWidth];
        for (int j = 0; j < imageHeight; j++)
        {
            for (int i = 0; i < imageWidth; i++)
            {
                var index = j * imageWidth + i;
                var found = levelMapping.Find(i=> i.color == pixels[index]);
                TileDescriptor tile = new TileDescriptor();
                if (found != null)
                {
                    tile.so = found;
                }
                else {
                    tile.so = invalidTile;
                }
                tiles[j,i] = tile;
            }
        }

        room.SetTiles(tiles);
        
        return room;
    }

    private void DrawRoom(Room room) {
        var width = room.GetWidth();
        var height = room.GetHeight();
        for (var j = 0; j < height; j++)
        {
            for (var i = 0; i < width; i++)
            {
                var position = new Vector2Int(i, j);
                var tile = room.GetTile(position);
                if (tile.so != null)
                {
                    var randomIndex = Random.Range(0, tile.so.prefabs.Count);
                    var randomPrefab = tile.so.prefabs[randomIndex];
                    var instanceTile = Instantiate(randomPrefab, new Vector2(i, j), Quaternion.identity, boardGameObject.transform);
                    tile.tile = instanceTile;
                }
            }
        }
    }

    private void ClearRoom(Room room)
    {
        for (var j = 0; j < room.GetHeight(); j++)
        {
            for (var i = 0; i < room.GetWidth(); i++)
            {
                var tile = room.GetTile(new Vector2Int(i,j));
                Destroy(tile.tile);
            }
        }
    }

    private GameObject CreatePlayerGameObject() {
        var player = Instantiate(playerPrefab, new Vector3(), Quaternion.identity, boardGameObject.transform);
        return player;
    }

    private GameObject CreateEnemyGameObject(Vector2Int startPos)
    {
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var enemy = Instantiate(enemyPrefab,
            new Vector3(startPos.x, startPos.y, 0), Quaternion.identity, boardGameObject.transform);
        return enemy;
    }
}
