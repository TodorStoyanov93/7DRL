using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverlayController : MonoBehaviour
{
    public static OverlayController Instance { get; private set; }

    public GameObject overlayPrefab;
    private GameObject overlayContainer;
    private GameObject highlightForCursor;
    private List<GameObject> overlays;
    private List<GameObject> targetOverlays;


    private void Awake()
    {
        Instance = this;
        
        overlays = new List<GameObject>();
        targetOverlays = new List<GameObject>();
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


    public void HighlightForCursor(Vector2Int position)
    {
        overlays.ForEach(i=>i.SetActive(true));
        Color color = Color.gray;
        var contains = overlays.Select(i => Helpers.RoundToVector2Int(i.transform.position)).Contains(position);
        if (contains) {
            var first = overlays.Find(i => Helpers.RoundToVector2Int(i.transform.position) == position);
            first.SetActive(false);
            color = Color.green;
        }
        targetOverlays.ForEach(i => i.SetActive(true));
        var contains2 = targetOverlays.Select(i => Helpers.RoundToVector2Int(i.transform.position)).Contains(position);
        if (contains2)
        {
            var first = targetOverlays.Find(i => Helpers.RoundToVector2Int(i.transform.position) == position);
            first.SetActive(false);
            color = Color.green;
        }

        if (highlightForCursor != null)
        {
            var oldPos = Helpers.RoundToVector2Int(highlightForCursor.transform.position);
            if ((position - oldPos).magnitude > 0.01f)
            {
                ClearHighlightForCursor();
                highlightForCursor = CreateOverlayTile(new Vector2Int(position.x, position.y),color);
            }
        }
        else {
            ClearHighlightForCursor();
            highlightForCursor = CreateOverlayTile(new Vector2Int(position.x, position.y),color);
        }
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
        overlays.Clear();

    }

    public void ClearTargetTiles()
    {
        foreach (var highlightForWalkable in targetOverlays)
        {
            if (highlightForWalkable != null)
            {
                Destroy(highlightForWalkable);
            }
        }
        targetOverlays.Clear();
    }

    public void OverlayWalkableTilesForPlayer() {
        var player = BoardManager.Instance.playerUnit.gameObject;
        var room = BoardManager.Instance.layout.startingRoom;
        var playerPosition = player.transform.position;
        Vector2Int playerPosInt = new Vector2Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y));

        List<Vector2Int> placesToCheck = new List<Vector2Int>() { 
            new Vector2Int(playerPosInt.x,playerPosInt.y-1),
            new Vector2Int(playerPosInt.x,playerPosInt.y+1),
            new Vector2Int(playerPosInt.x+1,playerPosInt.y),
            new Vector2Int(playerPosInt.x-1,playerPosInt.y)
        };


        foreach (var place in placesToCheck)
        {
            var tile = room.GetTile(place);
            if (tile != null)
            {
                if (room.GetTile(place).so.isWalkable)
                {
                    var overlay = CreateOverlayTile(place, Color.white);
                    overlays.Add(overlay);
                }
            }
        }

    }


    public void OverlayTargetTiles(List<Vector2Int> positions)
    {

        foreach (var pos in positions)
        {
            var overlay = CreateOverlayTile(pos, Color.red);
            targetOverlays.Add(overlay);
        }

    }

    public GameObject CreateOverlayTile(Vector2Int place,Color color) {
        var overlayGameObject = Instantiate(overlayPrefab, new Vector3(place.x, place.y, 0), Quaternion.identity, overlayContainer.transform);

        overlayGameObject.GetComponent<SpriteRenderer>().color = color;

        return overlayGameObject;

    }

}
