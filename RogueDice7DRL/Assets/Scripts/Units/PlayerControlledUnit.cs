using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledUnit : Unit
{
    public PlayerInputState playerInputState;

    public void SwitchPlayerInputState(PlayerInputState newState) {
        this.playerInputState = newState;
        if (playerInputState == PlayerInputState.Move)
        {
            OverlayController.Instance.OverlayWalkableTilesForPlayer();
        }
        else
        {

            OverlayController.Instance.ClearWalkableTiles();
            OverlayController.Instance.ClearHighlightForCursor();
        }
    }

    public void OnTileHover(Vector2Int hitGoPos) {
        OverlayController.Instance.HighlightForCursor(hitGoPos);
    }

    internal void OnTileClick(Vector2Int hitGoPos)
    {
        if (playerInputState == PlayerInputState.Move) { 
            GameplayController.Instance.StartCoroutine(MoveUnitCoroutine(this, hitGoPos));
        } else if (playerInputState == PlayerInputState.ChoseTarget) {
            if (IsValidTarget(clickedDice.chosenSide, hitGoPos))
            {
                clickedDice.chosenSide.OnUse(hitGoPos);
                clickedDice.isActive = false;
                PlayerUIManager.Instance.ResetCardView();
                SwitchPlayerInputState(PlayerInputState.Move);
            }
            else { 
                //Display popup that target is invalid
            }
            
        }
    }

    private bool IsValidTarget(SideData clickedDiceSide, Vector2Int hitGoPos)
    {
        var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
        List<Vector2Int> validTargets = clickedDiceSide.GetValidTargets(playerPos);
        return validTargets.Contains(hitGoPos);
    }


    IEnumerator MoveUnitCoroutine(Unit unit, Vector2Int position)
    {
        if (playerInputState != PlayerInputState.Move) {
            throw new Exception();
        }

        var unitPos = Helpers.RoundToVector2Int(unit.gameObject.transform.position);
        var diff = position - Helpers.RoundToVector2Int(unitPos);
        bool isIn1Range = diff.magnitude == 1.0f;
        bool isWalkable = BoardManager.Instance.layout.startingRoom.GetTile(position).so.isWalkable;

        if (isIn1Range && isWalkable)
        {
            InputController.Instance.StopWaitingForInput();
            SwitchPlayerInputState(PlayerInputState.Undefined);
            float duration = 0.5f;
            AnimationManager.Instance.PlayMoveAnimation(this);
            var smoothFollow = GameplayController.Instance.StartCoroutine(CameraController.Instance.FollowSmooth(BoardManager.Instance.playerUnit.gameObject, duration));
            var smoothMove = GameplayController.Instance.StartCoroutine(GameplayController.Instance.SmoothMove(BoardManager.Instance.playerUnit.gameObject, position, duration));
            yield return smoothFollow;
            yield return smoothMove;
            AnimationManager.Instance.StopMoveAnimation(this);
            TurnSystem.Instance.EndTurn();
        }
    }


    internal void OnRightClick()
    {
        if (playerInputState == PlayerInputState.Move)
        {
            AnimationManager.Instance.PlayAttackAnimation(this);
        }
        else if (playerInputState == PlayerInputState.ChoseTarget) {
            PlayerUIManager.Instance.CancelCurrentDice();
            SwitchPlayerInputState(PlayerInputState.Move);
        }
    }

    public override void Turn()
    {
        SwitchPlayerInputState(PlayerInputState.Move);
        InputController.Instance.WaitForInput(this);
    }

    ActivatableDice clickedDice;
    internal void DiceClicked(ActivatableDice activatableDice)
    {
        if (activatableDice.isActive)
        {
            var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
            var validTargets = activatableDice.chosenSide.GetValidTargets(playerPos);
            Debug.Log(validTargets);
            //Overlay validTargets
            SwitchPlayerInputState(PlayerInputState.ChoseTarget);
            clickedDice = activatableDice;
        }
        else
        {
            Debug.Log("Dice must be rolled before being used");
            PlayerUIManager.Instance.CancelCurrentDice();
        }
    }
}
