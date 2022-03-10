using System;
using System.Collections;
using System.Collections.Generic;
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
        bool isWalkable = BoardManager.Instance.layout.startingRoom.GetTile(moveTargetPosition).so.isWalkable;

        if (isIn1Range && isWalkable)
        {
            return true;
        }
        return false;
    }

    public bool IsValidTarget(Vector2Int unitPosition, SideData clickedDiceSide, Vector2Int hitGoPos)
    {
        List<Vector2Int> validTargets = clickedDiceSide.GetValidTargets(unitPosition);
        return validTargets.Contains(hitGoPos);
    }
}
