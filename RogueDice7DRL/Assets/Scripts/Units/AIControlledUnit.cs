using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControlledUnit : Unit
{
    public override void Turn()
    {
        var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
        var distance = (playerPos - this.GetVector2IntPosition()).magnitude;

        if (distance < 3)
        {
            var c = DealDamageCoroutine();
            ActionExecutor.Instance.StartCoroutine(c);
        }
        else { 
        
            var randomDirection = new List<Vector2Int>()
            {
                new Vector2Int(0,-Random.Range(1,3)),
                new Vector2Int(0,Random.Range(1,3)),
                new Vector2Int(-Random.Range(1,3),0),
                new Vector2Int(-Random.Range(1,3),0)
            }[Random.Range(0,4)];
            Vector2Int targetPosition = Helpers.RoundToVector2Int(this.gameObject.transform.position) + randomDirection;

            var coroutine = MoveUnitCoroutine(this, targetPosition);
            GameplayController.Instance.StartCoroutine(coroutine);
        }



        
    }

    IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int position)
    {
        var unitPos = Helpers.RoundToVector2Int(unit.gameObject.transform.position);
        var diff = position - Helpers.RoundToVector2Int(unitPos);
        bool isIn2Range = diff.magnitude <= 2.0f;
        var tile = BoardManager.Instance.room.GetTile(position);
        if (tile!= null) { 
            bool isWalkable = tile.so.isWalkable;

            if (isIn2Range && isWalkable)
            {
                float duration = 0.5f;
                AnimationManager.Instance.PlayMoveAnimation(unit);
                var smoothFollow = GameplayController.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(this.gameObject, duration));
                var smoothMove = GameplayController.Instance.StartCoroutine(GameplayController.Instance.SmoothMove(this.gameObject, position, duration));
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
