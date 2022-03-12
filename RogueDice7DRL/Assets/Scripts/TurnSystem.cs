using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Unit> units;
    public Unit currentUnit;
    private int currentActorIndex;
    public static TurnSystem Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        ResetTurnSystem();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetTurnSystem()
    {
        units = new List<Unit>();
        currentUnit = null;
        currentActorIndex = 0;
    }

    internal void BeginFirstTurn()
    {
        EnsureAtleast1Actor();
        SetCurrentActorFirst();
        StartCoroutine(BeginTurn());
    }

    private void SetCurrentActorFirst()
    {
        currentActorIndex = 0;
        currentUnit = units[currentActorIndex];
    }

    private void SetCurrentActorNext() {
        currentActorIndex++;
        currentActorIndex %= units.Count;
        currentUnit = units[currentActorIndex];
    }


    private void EnsureAtleast1Actor()
    {
        if (units.Count == 0) {
            Debug.Log("No actors in scene");
            throw new Exception("No actors in scene");
        } 
    }

    IEnumerator BeginTurn() {
        if (!CameraController.Instance.CameraIsOnGameObject(currentUnit.gameObject)) {
            yield return StartCoroutine(CameraController.Instance.FollowSmooth(currentUnit.gameObject, 0.5f));
        }
        currentUnit.Turn();
    }

    public void EndTurn()
    {
        if (currentUnit == BoardManager.Instance.playerUnit) {
            BoardManager.Instance.playerTurns++;
        }

        if (units.Count > 0) { 
            SetCurrentActorNext();
            StartCoroutine(BeginTurn());
        }
    }

    internal void RemoveUnit(Unit unit)
    {
        for (var i = 0; i < units.Count; i++) {
            var curr = units[i];
            if (curr == unit)
            {
                units.RemoveAt(i);

                if (currentActorIndex > i)
                {
                    currentActorIndex--;
                }
                break;
            }
        }
    }
}
