using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControlledUnit : Unit
{
    public override void Turn()
    {
        var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
        var distance = (playerPos - this.GetVector2IntPosition()).magnitude;

        if (distance < 1.5f)
        {
            var c = DealDamageCoroutine();
            ActionExecutor.Instance.StartCoroutine(c);
        }
        else { 
        

            var playerDiff = this.GetVector2IntPosition() - BoardManager.Instance.playerUnit.GetVector2IntPosition();

            Debug.Log(playerDiff);
            Vector2Int targetPosition = Vector2Int.zero;
            if (Mathf.Abs(playerDiff.x) > Mathf.Abs(playerDiff.y))
            {
                if (playerDiff.x > 0)
                {
                    targetPosition = this.GetVector2IntPosition() + new Vector2Int(-1, 0);
                }
                else
                {
                    targetPosition = this.GetVector2IntPosition() + new Vector2Int(1, 0);
                }
            }
            else {
                if (playerDiff.y > 0)
                {
                    targetPosition = this.GetVector2IntPosition() + new Vector2Int(0,-1);
                }
                else
                {
                    targetPosition = this.GetVector2IntPosition() + new Vector2Int(0, 1);
                }
            }

            var coroutine = MoveUnitCoroutine(this, targetPosition);
            ActionExecutor.Instance.StartCoroutine(coroutine);
        }
        
    }

    IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int position)
    {
        var unitPos = Helpers.RoundToVector2Int(unit.gameObject.transform.position);
        var diff = position - unitPos;
        bool isIn2Range = diff.magnitude <= 2.0f;
        var tile = BoardManager.Instance.room.GetTile(position);
        if (tile!= null) { 
            bool isWalkable = tile.so.isWalkable;

            if (isIn2Range && isWalkable)
            {
                float duration = 0.5f;
                AnimationManager.Instance.PlayMoveAnimation(unit);
                var smoothFollow = ActionExecutor.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(this.gameObject, duration));
                var smoothMove = ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.SmoothMove(this.gameObject, position, duration));
                yield return smoothFollow;
                yield return smoothMove;
                AnimationManager.Instance.StopMoveAnimation(unit);
                TurnSystem.Instance.EndTurn();
            }
            else {
                TurnSystem.Instance.EndTurn();
            }
        }
        else
        {
            TurnSystem.Instance.EndTurn();
        }
    }

    IEnumerator DealDamageCoroutine() {
        var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
        yield return ActionExecutor.Instance.DealDamageInTile(playerPos,1);
        TurnSystem.Instance.EndTurn();

    }
}
