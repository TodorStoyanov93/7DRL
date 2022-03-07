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
    int depth;
    private CameraController cameraController;

    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;

    public GameObject player;

    public LevelLayout layout;

    public List<GameObject> enemies;

    public static BoardManager Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        cameraController = GetComponent<CameraController>();
    }
    void Start()
    {
        this.boardGameObject = GameObject.Find("Board");
        int seed = 5;
        Random.InitState(seed);
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
        Instance.player = CreatePlayerGameObject(startPos);
        Unit playerUnit = new PlayerControlledUnit();
        playerUnit.gameObject = player;
        TurnSystem.Instance.units.Add(playerUnit);

        for (var i = 0; i < 5; i++) { 
            var randomWalkablePos = GetRandomWalkablePosition(layout.startingRoom);
            var enemy = CreateEnemyGameObject(randomWalkablePos);
            Instance.enemies.Add(enemy);
            Unit enemyUnit = new AIControlledUnit();
            enemyUnit.gameObject = enemy;
            TurnSystem.Instance.units.Add(enemyUnit);
        }

        CameraController.Instance.SnapTo(player);
        TurnSystem.Instance.BeginFirstTurn();
    }

    private static Vector2Int GetPlayerStartingPosition(Room room) {
        var width = room.GetWidth();
        var height = room.GetHeight();
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++)
            {
                if (room.GetTile(i, j).so.isWalkable) {
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

    private static Room GenerateRoom() {

        var imageIndex = Random.Range(0, Instance.levelTextures.Count);
        var image = Instance.levelTextures[imageIndex];
        Room room = GenerateRoomFromTexture2D(image);
        return room;
    }

    private static Room GenerateRoomFromTexture2D(Texture2D source)
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
                var found = Instance.levelMapping.Find(i=> i.color == pixels[index]);
                TileDescriptor tile = new TileDescriptor();
                if (found != null)
                {
                    tile.so = found;
                }
                else {
                    tile.so = Instance.invalidTile;
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
                var tile = room.GetTile(i,j);
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

    private static GameObject CreatePlayerGameObject(Vector2Int startPos) {
        var player = Instantiate(Instance.playerPrefab, 
            new Vector3(startPos.x, startPos.y, 0), Quaternion.identity, Instance.boardGameObject.transform);
        return player;
    }

    private static GameObject CreateEnemyGameObject(Vector2Int startPos)
    {
        var enemyPrefab = Instance.enemyPrefabs[Random.Range(0, Instance.enemyPrefabs.Count)];
        var enemy = Instantiate(enemyPrefab,
            new Vector3(startPos.x, startPos.y, 0), Quaternion.identity, Instance.boardGameObject.transform);
        return enemy;
    }
}
