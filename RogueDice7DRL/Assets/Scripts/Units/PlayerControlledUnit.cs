using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledUnit : Unit
{
    public void OnTileHover(Vector2Int hitGoPos) {
        OverlayController.Instance.HighlightForCursor(hitGoPos);
    }

    internal void OnTileClick(Vector2Int hitGoPos)
    {
        GameplayController.Instance.StartCoroutine(MoveUnitCoroutine(this, hitGoPos));
    }

    IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int position)
    {
        var unitPos = Helpers.RoundToVector2Int(unit.gameObject.transform.position);
        var diff = position - Helpers.RoundToVector2Int(unitPos);
        bool isIn1Range = diff.magnitude == 1.0f;
        bool isWalkable = BoardManager.Instance.layout.startingRoom.GetTile(position.x, position.y).so.isWalkable;

        if (isIn1Range && isWalkable)
        {
            InputController.Instance.StopWaitingForInput();
            OverlayController.Instance.ClearWalkableTiles();
            OverlayController.Instance.ClearHighlightForCursor();
            float duration = 0.5f;
            AnimationManager.Instance.PlayMoveAnimation(this);
            var smoothFollow = GameplayController.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(BoardManager.Instance.player, duration));
            var smoothMove = GameplayController.Instance.StartCoroutine(GameplayController.Instance.SmoothMove(BoardManager.Instance.player, position, duration));
            yield return smoothFollow;
            yield return smoothMove;
            AnimationManager.Instance.StopMoveAnimation(this);
            TurnSystem.Instance.EndTurn();
        }
    }


    internal void OnRightClick()
    {
        AnimationManager.Instance.PlayAttackAnimation(this);
    }

    public override void Turn()
    {
        OverlayController.Instance.OverlayWalkableTilesForPlayer();

        InputController.Instance.WaitForInput(this);
    }
}
