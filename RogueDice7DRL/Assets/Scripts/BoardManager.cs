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

    public GameObject player;

    public LevelLayout layout;

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
        var startingRoom = GenerateRoom();
        layout.startingRoom = startingRoom;
        DrawRoom(layout.startingRoom);
        var startPos = GetPlayerStartingPosition(layout.startingRoom);
        CreatePlayer(startPos);



        this.layout = layout;
        Actor playerActor = new ActorPlayer();

        //TurnSystem.Instance.actors.Add(playerActor);
        //TurnSystem.Instance.BeginFirstTurn();
        GameplayController.Instance.BeginPlayerTurn();

        
    }

    private static Tuple<int, int> GetPlayerStartingPosition(Room room) {
        var width = room.GetWidth();
        var height = room.GetHeight();
        for (var j = 0; j < height; j++) {
            for (var i = 0; i < width; i++)
            {
                if (room.GetTile(i, j).so.isWalkable) {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        return null;
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

    private static void CreatePlayer(Tuple<int, int> startPos) {

        var player = Instantiate(Instance.playerPrefab, 
            new Vector3(startPos.Item1, startPos.Item2, 0), Quaternion.identity, Instance.boardGameObject.transform);
        Instance.player = player;
    }
}
