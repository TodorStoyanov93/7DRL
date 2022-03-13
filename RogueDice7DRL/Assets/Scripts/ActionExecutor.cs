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

    public IEnumerator ApplyShieldInTile(Vector2Int position, int shield) {
        yield return StartCoroutine(ApplyShieldInTileCoroutine(position, shield));
    }

    IEnumerator ApplyShieldInTileCoroutine(Vector2Int position, int shield  )
    {
        var tileAnimationCoroutine = StartCoroutine(AnimationManager.Instance.PlayShieldAnimationCoroutine(position));

        var enemyHit = BoardManager.Instance.enemyUnits.Find(enemy => enemy.GetVector2IntPosition() == position);
        if (enemyHit != null)
        {
            yield return StartCoroutine(ApplyShield(enemyHit, shield));
        }
        else
        {
            var playerIsHit = BoardManager.Instance.playerUnit.GetVector2IntPosition() == position;
            if (playerIsHit)
            {
                yield return StartCoroutine(ApplyShield(BoardManager.Instance.playerUnit, shield));
            }
        }

        yield return tileAnimationCoroutine;
    }


    IEnumerator ApplyShield(Unit unit, int shield)
    {
        unit.SetShield(shield);

        yield break;
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
                unit.SetShield(0);

            }
            else
            {
                remainingDamage = 0;
                unit.SetShield(damage - unit.shield);

            }
        } else {
            remainingDamage = damage;
        }

        if (unit.currentHealth > 0)
        {
            if (remainingDamage < unit.currentHealth)
            {
                unit.SetHealth(unit.currentHealth - remainingDamage);
            }
            else
            {
                unit.SetHealth(0);
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
        float duration = 0.5f;
        AnimationManager.Instance.PlayMoveAnimation(unit);
        AnimationManager.Instance.TurnSprite(unit, moveTargetPosition);
        var smoothFollow = StartCoroutine(CameraController.Instance.FollowSmooth(BoardManager.Instance.playerUnit.gameObject, duration));
        var smoothMove = StartCoroutine(ActionExecutor.Instance.SmoothMove(BoardManager.Instance.playerUnit.gameObject, moveTargetPosition, duration));
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

    public IEnumerator SmoothMove(GameObject gameObject, Vector2 desiredPosition, float duration)
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


    public IEnumerator RollCoroutine() {
        List<Coroutine> coroutines = new List<Coroutine>();
        for (int i = 0; i < BoardManager.Instance.playerUnit.dices.Count; i++) {
            var currDice = BoardManager.Instance.playerUnit.dices[i];
            currDice.isActive = false;
            var c = StartCoroutine(RollSingleDice(currDice));
            coroutines.Add(c);
        }
        

        foreach (var coroutine in coroutines) {
            yield return coroutine;
        }

    }
    IEnumerator RollSingleDice(ActivatableDice activatableDice) {
        for (var i = 0; i < 10; i++) { 
            activatableDice.ChooseRandomSide();
            PlayerUIManager.Instance.RefreshDiceUi(activatableDice);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.50f));

        }
        activatableDice.isActive = true;
        PlayerUIManager.Instance.RefreshDiceUi(activatableDice);
        yield break;
    }
}
