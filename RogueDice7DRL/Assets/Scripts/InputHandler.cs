using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerInputState playerInputState;

    public static InputHandler Instance { get; private set; }

    private Unit _playerUnit;
    private Unit PlayerUnit { get {
            if (_playerUnit == null) {//may become invalid when changing rooms
                _playerUnit = BoardManager.Instance.playerUnit;
            }
            return _playerUnit;
        }
    }

    private ActivatableDice selectedDice;

    void Awake() {
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

    public void OnTileHover(Vector2Int hitGoPos)
    {
        OverlayController.Instance.HighlightForCursor(hitGoPos);
    }

    internal void OnNoTileHovered()
    {
        OverlayController.Instance.ClearHighlightForCursor();
    }

    internal void OnTileClick(Vector2Int hitGoPos) //Check if enemy tile clicked

    {
        if (playerInputState == PlayerInputState.Default)
        {
            Vector2Int unitPosition = PlayerUnit.GetVector2IntPosition();
            if (ActionExecutor.Instance.IsValidMove(unitPosition, hitGoPos))
            {
                StartCoroutine(MovePlayer(hitGoPos));
            }
            else
            {
                Debug.Log("Cant move there");
            }
        }
        else if (playerInputState == PlayerInputState.ChoseTarget)
        {
            var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
            if (ActionExecutor.Instance.IsValidTarget(playerPos, selectedDice.chosenSide, hitGoPos))
            {
                
                
                selectedDice.chosenSide.OnUse(hitGoPos);
                selectedDice.isActive = false;
                PlayerUIManager.Instance.RefreshCurrentDiceActive();
                PlayerUIManager.Instance.CancelCurrentDice();
                PlayerUIManager.Instance.HideDiceTooltip();
                selectedDice = null;

                OverlayController.Instance.ClearTargetTiles();
                playerInputState = PlayerInputState.Default;
                OverlayController.Instance.OverlayWalkableTilesForPlayer();

            }
            else
            {
                Debug.Log("Invalid target");
                //Display popup that target is invalid
            }

        }
        else if (playerInputState == PlayerInputState.ChoseCard) {

        }
    }

    IEnumerator MovePlayer(Vector2Int moveTarget) {
        playerInputState = PlayerInputState.Undefined;
        //InputController.Instance.StopWaitingForInput();
        OverlayController.Instance.ClearWalkableTiles();
        yield return StartCoroutine(ActionExecutor.Instance.MoveUnitCoroutine(PlayerUnit, moveTarget));

        TurnSystem.Instance.EndTurn();

    }




    internal void OnRightClick()
    {
        if (playerInputState == PlayerInputState.Default)
        {
            AnimationManager.Instance.PlayAttackAnimation(PlayerUnit);
        }
        else if (playerInputState == PlayerInputState.ChoseTarget)
        {
            
            PlayerUIManager.Instance.CancelCurrentDice();
            PlayerUIManager.Instance.HideDiceTooltip();
            OverlayController.Instance.ClearTargetTiles();

            playerInputState = PlayerInputState.Default;
            OverlayController.Instance.OverlayWalkableTilesForPlayer();
        }
    }

    internal void DiceClicked(ActivatableDice activatableDice)
    {
        if (playerInputState == PlayerInputState.Default)
        {
            if (activatableDice.isActive)
            {
                OverlayController.Instance.ClearWalkableTiles();
                var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
                var validTargets = activatableDice.chosenSide.GetValidTargets(playerPos);
                playerInputState = PlayerInputState.ChoseTarget;
                OverlayController.Instance.OverlayTargetTiles(validTargets);
                selectedDice = activatableDice;
                PlayerUIManager.Instance.ShowDiceTooltip(activatableDice);
            }
            else
            {
                Debug.Log("Dice must be rolled before being used");
                PlayerUIManager.Instance.CancelCurrentDice();

            }
        }
        else if (playerInputState == PlayerInputState.ChoseTarget)
        {
            if (activatableDice.isActive)
            {
                OverlayController.Instance.ClearTargetTiles();
                var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
                var validTargets = activatableDice.chosenSide.GetValidTargets(playerPos);

                OverlayController.Instance.OverlayTargetTiles(validTargets);
                selectedDice = activatableDice;

                PlayerUIManager.Instance.ShowDiceTooltip(activatableDice);
            }
            else
            {
                Debug.Log("Dice must be rolled before being used");
                PlayerUIManager.Instance.CancelCurrentDice();
            }
        }
        else if (playerInputState == PlayerInputState.Undefined)
        {
            Debug.Log(" DiceClicked State Undefined");
        }
    }


    public void EndTurnClicked() {

    }

    public void RollClicked() {
        Debug.Log("Roll");
    }

    public void CheatClicked() { 
        
    }

    public void TurnBeginsForPlayer() {
        if (playerInputState == PlayerInputState.Undefined) { 
            if (selectedDice == null)
            {
                playerInputState = PlayerInputState.Default;
                OverlayController.Instance.OverlayWalkableTilesForPlayer();

            }
            else {
                playerInputState = PlayerInputState.ChoseTarget;
                var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
                var validTargets = selectedDice.chosenSide.GetValidTargets(playerPos);
                OverlayController.Instance.OverlayTargetTiles(validTargets);
            }
        }
    }

    public void Reset() {
        playerInputState = PlayerInputState.Undefined;
    }

}
