using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControlledUnit : Unit
{
    public override void Turn()
    {
        

        var randomDirection = new List<Vector2Int>()
        {
            new Vector2Int(0,-Random.Range(1,2)),
            new Vector2Int(0,Random.Range(1,2)),
            new Vector2Int(-Random.Range(1,2),0),
            new Vector2Int(-Random.Range(1,2),0)
        }[Random.Range(0,4)];
        Vector2Int targetPosition = Helpers.RoundToVector2Int(this.gameObject.transform.position) + randomDirection;

        var coroutine = MoveUnitCoroutine(this, targetPosition);
        GameplayController.Instance.StartCoroutine(coroutine);
    }

    IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int position)
    {
        var unitPos = Helpers.RoundToVector2Int(unit.gameObject.transform.position);
        var diff = position - Helpers.RoundToVector2Int(unitPos);
        bool isIn2Range = diff.magnitude <= 1.0f;
        bool isWalkable = BoardManager.Instance.layout.startingRoom.GetTile(position.x, position.y).so.isWalkable;

        if (isIn2Range && isWalkable)
        {
            //InputController.Instance.StopWaitingForInput();
            //OverlayController.Instance.ClearWalkableTiles();
            //OverlayController.Instance.ClearHighlightForCursor();
            float duration = 0.5f;
            //AnimationManager.Instance.PlayMoveAnimation();//for bat
            var smoothFollow = GameplayController.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(this.gameObject, duration));
            var smoothMove = GameplayController.Instance.StartCoroutine(GameplayController.Instance.SmoothMove(this.gameObject, position, duration));
            yield return smoothFollow;
            yield return smoothMove;
            //AnimationManager.Instance.StopMoveAnimation();
            TurnSystem.Instance.EndTurn();
        }
        else {
            TurnSystem.Instance.EndTurn();
        }
    }
}
