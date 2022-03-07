using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{

    public static GameplayController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginPlayerTurn() {
        CameraController.Instance.SnapTo(BoardManager.Instance.player);

        var player = BoardManager.Instance.player;



        OverlayController.Instance.OverlayWalkableTiles(player);

        InputController.Instance.WaitForInput();
    }

    public void EndPlayerTurn() {
        BeginPlayerTurn();
    }

    IEnumerator BeginTurnAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        AnimationManager.Instance.StopMoveAnimation();
        BeginPlayerTurn();
        yield break;
    }

    private void MoveCharacter(Vector2 position)
    {
        InputController.Instance.StopWaitingForInput();
        OverlayController.Instance.ClearWalkableTiles();
        OverlayController.Instance.ClearHighlightForCursor();
        float duration = 0.5f;
        AnimationManager.Instance.PlayMoveAnimation();
        StartCoroutine(CameraController.Instance.FollowSmooth(BoardManager.Instance.player, duration));
        StartCoroutine(SmoothMove(BoardManager.Instance.player, position, duration));
        StartCoroutine(BeginTurnAfter(duration));
    }

    IEnumerator SmoothMove(GameObject gameObject,Vector2 desiredPosition, float duration)
    {
        float timeElapsed = 0f;
        Vector3 start = gameObject.transform.position;
        while (timeElapsed <= duration)
        {

            var lerpedValue = Vector3.Lerp(start, desiredPosition, timeElapsed / duration);

            gameObject.transform.position = lerpedValue;
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        gameObject.transform.position = desiredPosition;

        yield break;
    }

    public void BeginEnemyTurn() { 
    
    }

    internal void PlayerInputMove(Vector2Int position)
    {
        var playerPos = BoardManager.Instance.player.transform.position;
        var diff = position - new Vector2(playerPos.x, playerPos.y);
        bool isIn1Range = diff.magnitude == 1.0f;
        bool isWalkable = BoardManager.Instance.layout.startingRoom.GetTile(position.x,position.y).so.isWalkable;

        if (isIn1Range && isWalkable) {
            MoveCharacter(position);
        }
    }
}
