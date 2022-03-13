using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    private PlayerInputState playerInputState;

    public static InputHandler Instance { get; private set; }

    private ActivatableDice selectedDice;

    private GameObject rollButton;

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
            Vector2Int unitPosition = BoardManager.Instance.playerUnit.GetVector2IntPosition();
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
                SetRollButtonEnabled(true);
            }
            else
            {
                Debug.Log("Invalid target");
                //Display popup that target is invalid
            }

        }
        else if (playerInputState == PlayerInputState.Undefined) {

        }
    }

    IEnumerator MovePlayer(Vector2Int moveTarget) {
        playerInputState = PlayerInputState.Undefined;
        SetRollButtonEnabled(false);
        //InputController.Instance.StopWaitingForInput();
        OverlayController.Instance.ClearWalkableTiles();
        yield return StartCoroutine(ActionExecutor.Instance.MoveUnitCoroutine(BoardManager.Instance.playerUnit, moveTarget));

        TurnSystem.Instance.EndTurn();
    }




    internal void OnRightClick()
    {
        if (playerInputState == PlayerInputState.Default)
        {

        }
        else if (playerInputState == PlayerInputState.ChoseTarget)
        {

            PlayerUIManager.Instance.CancelCurrentDice();
            PlayerUIManager.Instance.HideDiceTooltip();
            OverlayController.Instance.ClearTargetTiles();

            playerInputState = PlayerInputState.Default;
            SetRollButtonEnabled(true);
            OverlayController.Instance.OverlayWalkableTilesForPlayer();
        }
        else if (playerInputState == PlayerInputState.Undefined) { 
        
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
                SetRollButtonEnabled(false);
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
            PlayerUIManager.Instance.CancelCurrentDice();
        }
    }


    public void EndTurnClicked() {

    }

    public void RollClicked() {
        StartCoroutine(Roll());
    }

    IEnumerator Roll() {
        if (playerInputState == PlayerInputState.Default) {
            playerInputState = PlayerInputState.Undefined;
            OverlayController.Instance.ClearWalkableTiles();
            SetRollButtonEnabled(false);
            yield return StartCoroutine(ActionExecutor.Instance.RollCoroutine());

            TurnSystem.Instance.EndTurn();
        }
    }



    public void CheatClicked() { 
        
    }

    public void TurnBeginsForPlayer() {
        if (playerInputState == PlayerInputState.Undefined) {
            if (selectedDice == null)
            {
                playerInputState = PlayerInputState.Default;
                OverlayController.Instance.OverlayWalkableTilesForPlayer();
                SetRollButtonEnabled(true);
            }
            else {
                playerInputState = PlayerInputState.ChoseTarget;
                var playerPos = BoardManager.Instance.playerUnit.GetVector2IntPosition();
                var validTargets = selectedDice.chosenSide.GetValidTargets(playerPos);
                OverlayController.Instance.OverlayTargetTiles(validTargets);
                SetRollButtonEnabled(false);
            }
        }
    }

    public void ResetInputHandler() {
        playerInputState = PlayerInputState.Undefined;
        SetRollButtonEnabled(false);
    }

    private void SetRollButtonEnabled(bool enabled) {
        var playerUi = GameObject.Find("PlayerUI");
        var rollContainer = playerUi.transform.Find("RollContainer");
        var rollButton = rollContainer.transform.Find("RollButton");

        Button button = rollButton.GetComponent<Button>();
        button.interactable = enabled;
    }

}
