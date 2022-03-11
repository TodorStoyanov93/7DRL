using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionExecutor : MonoBehaviour
{
    public static ActionExecutor Instance;

    void Awake() {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public IEnumerator DealDamageInTile(Vector2Int position, int damage)
    {
        yield return StartCoroutine(DealDamageInTileCoroutine(position,damage));
    }

    IEnumerator DealDamage(Unit unit,int damage) {
        var remainingDamage = 0;
        if (unit.shield > 0)
        {
            if (damage >= unit.shield)
            {
                remainingDamage = damage - unit.shield;
                unit.shield = 0;

            }
            else
            {
                remainingDamage = 0;
                unit.shield = damage - unit.shield;

            }
        }

        if (unit.currentHealth > 0)
        {
            if (remainingDamage > unit.currentHealth)
            {
                unit.currentHealth -= remainingDamage;
            }
            else
            {
                unit.currentHealth = 0;
                yield return StartCoroutine(MakeUnitDie(unit));
            }
        }

        yield break;
    }

    IEnumerator MakeUnitDie(Unit unit) {
        AnimationManager.Instance.PlayDeadAnimation(unit);
        yield return new WaitForSeconds(0.5f);
        BoardManager.Instance.MakeUnitDie(unit);
        yield break;
    }


    IEnumerator DealDamageInTileCoroutine(Vector2Int position, int damage) {
        var tileAnimationCoroutine = StartCoroutine(AnimationManager.Instance.PlayBlastAnimationCoroutine(position));

        var enemyHit = BoardManager.Instance.enemyUnits.Find(enemy => enemy.GetVector2IntPosition() == position);
        if (enemyHit != null)
        {
            yield return StartCoroutine(DealDamage(enemyHit, damage));
        }
        else { 
            var playerIsHit = BoardManager.Instance.playerUnit.GetVector2IntPosition() == position;
            if (playerIsHit) {
                yield return StartCoroutine(DealDamage(BoardManager.Instance.playerUnit, damage));
            }
        }

        yield return tileAnimationCoroutine;
    }


    // Update is called once per frame
    void Update()
    {
        
    }



    public IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int moveTargetPosition)
    {
        if (!IsValidMove(unit.GetVector2IntPosition(), moveTargetPosition)) {
            Debug.Log("Move is Invalid");
            throw new Exception("Move is Invalid");
        }

        //SwitchPlayerInputState(PlayerInputState.Undefined);
        float duration = 0.5f;
        AnimationManager.Instance.PlayMoveAnimation(unit);
        AnimationManager.Instance.TurnSprite(unit, moveTargetPosition);
        var smoothFollow = GameplayController.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(BoardManager.Instance.playerUnit.gameObject, duration));
        var smoothMove = GameplayController.Instance.StartCoroutine(GameplayController.Instance.SmoothMove(BoardManager.Instance.playerUnit.gameObject, moveTargetPosition, duration));
        yield return smoothFollow;
        yield return smoothMove;
        AnimationManager.Instance.StopMoveAnimation(unit);
        yield break;
    }

    public bool IsValidMove(Vector2Int unitPosition, Vector2Int moveTargetPosition)
    {
        var diff = unitPosition - moveTargetPosition;
        bool isIn1Range = diff.magnitude == 1.0f;
        var tile = BoardManager.Instance.room.GetTile(moveTargetPosition);
        if (tile != null)
        {
            bool isWalkable = tile.so.isWalkable;

            if (isIn1Range && isWalkable)
            {
                return true;
            }
            return false;
        }
        else {  
            return false;
        }
    }

    public bool IsValidTarget(Vector2Int unitPosition, SideData clickedDiceSide, Vector2Int hitGoPos)
    {
        List<Vector2Int> validTargets = clickedDiceSide.GetValidTargets(unitPosition);
        return validTargets.Contains(hitGoPos);
    }
}
