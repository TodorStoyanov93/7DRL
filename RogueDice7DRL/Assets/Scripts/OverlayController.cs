using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayController : MonoBehaviour
{
    public static OverlayController Instance { get; private set; }

    public GameObject overlayPrefab;
    private GameObject overlayContainer;

    List<GameObject> overlays;

    private void Awake()
    {
        Instance = this;
        
        overlays = new List<GameObject>();

        overlayContainer = new GameObject("OverlayContainer");

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject highlightForCursor;
    public void HighlightForCursor(Vector2Int position)
    {
        ClearHighlightForCursor();
        highlightForCursor = CreateOverlayTile(new Vector2Int(position.x, position.y));
    }


    public void ClearHighlightForCursor()
    {
        if (highlightForCursor != null)
        {
            Destroy(highlightForCursor);
        }
        highlightForCursor = null;
    }

    public void ClearWalkableTiles() {
        foreach (var highlightForWalkable in overlays) { 
            if (highlightForWalkable != null)
            {
                Destroy(highlightForWalkable);
            }
        }
        overlays = new List<GameObject>();

    }

    public void OverlayWalkableTiles(GameObject player) {
        var room = BoardManager.Instance.layout.startingRoom;
        var playerPosition = player.transform.position;
        Vector2Int playerPosInt = new Vector2Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y));

        List<Vector2Int> placesToCheck = new List<Vector2Int>() { 
            new Vector2Int(playerPosInt.x,playerPosInt.y-1),
            new Vector2Int(playerPosInt.x,playerPosInt.y+1),
            new Vector2Int(playerPosInt.x+1,playerPosInt.y),
            new Vector2Int(playerPosInt.x-1,playerPosInt.y)
        };

        foreach (var place in placesToCheck) {
            if (place.y >= 0 && place.y < room.GetHeight() && place.x >= 0 && place.x < room.GetWidth())
            {
                if (room.GetTile(place.x,place.y).so.isWalkable) {
                    var overlay = CreateOverlayTile(place);
                    overlays.Add(overlay);
                }
            }
        }

    }

    public GameObject CreateOverlayTile(Vector2Int place) {
        return Instantiate(overlayPrefab, new Vector3(place.x, place.y, 0), Quaternion.identity, overlayContainer.transform);
    }

}