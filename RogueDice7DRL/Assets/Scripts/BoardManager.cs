using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public List<Texture2D> levelTextures;
    public List<TileScriptableObject> levelMapping;

    public TileScriptableObject invalidTile;

    private GameObject boardGameObject;

    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;

    public LevelLayout layout;

    public Unit playerUnit;

    public List<Unit> enemyUnits;

    public static BoardManager Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        this.boardGameObject = GameObject.Find("Board");
        int seed = 5;
        Random.InitState(seed);
        enemyUnits = new List<Unit>();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        var layout = GetEmptyLayout();
        this.layout = layout;
        var startingRoom = GenerateRoom();
        layout.startingRoom = startingRoom;

        DrawRoom(layout.startingRoom);


        var startPos = GetPlayerStartingPosition(layout.startingRoom);
        var player = CreatePlayerGameObject(startPos);
        playerUnit = new PlayerControlledUnit();
        playerUnit.gameObject = player;
        TurnSystem.Instance.units.Add(playerUnit);

        for (var i = 0; i < 7; i++) { 
            DiceData randomDice = DiceManager.Instance.GetRandomDiceFromList();
        
            ActivatableDice activatableDice = ActivatableDice.CreateActivatableDice(true,randomDice);
            playerUnit.dices.Add(activatableDice);
        }

        for (var i = 0; i < 5; i++) { 
            var randomWalkablePos = GetRandomWalkablePosition(layout.startingRoom);
            var enemy = CreateEnemyGameObject(randomWalkablePos);
            Unit enemyUnit = new AIControlledUnit();
            enemyUnit.gameObject = enemy;
            TurnSystem.Instance.units.Add(enemyUnit);
            enemyUnits.Add(enemyUnit);
        }

        CameraController.Instance.SnapTo(player);

        PlayerUIManager.Instance.ClearCardView();
        PlayerUIManager.Instance.DrawCardView();


        PlayerUIManager.Instance.EnablePlayerUi();

        InputController.Instance.WaitForInput();

        TurnSystem.Instance.BeginFirstTurn();
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
            var tilePosToCheck = new Vector2Int(Random.Range(0, width - 1), Random.Range(0, height - 1));

            if (room.GetTile(tilePosToCheck).so.isWalkable) {
                return tilePosToCheck;
            }
            tries--;
        }
        return new Vector2Int();
    }


    private static LevelLayout GetEmptyLayout() {
        LevelLayout layout = new LevelLayout();
        return layout;
    }

    private Room GenerateRoom() {

        var imageIndex = Random.Range(0, levelTextures.Count);
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
                    var randomIndex = Random.Range(0, tile.so.prefabs.Count - 1);
                    var randomPrefab = tile.so.prefabs[randomIndex];
                    var instanceTile = Instantiate(randomPrefab, new Vector2(i, j), Quaternion.identity, boardGameObject.transform);
                    tile.tile = instanceTile;
                }
            }
        }
    }

    private GameObject CreatePlayerGameObject(Vector2Int startPos) {
        var player = Instantiate(playerPrefab, 
            new Vector3(startPos.x, startPos.y, 0), Quaternion.identity, boardGameObject.transform);
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
