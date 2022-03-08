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
        units = new List<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            StartCoroutine(CameraController.Instance.FollowSmooth(currentUnit.gameObject, 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        currentUnit.Turn();
    }

    public void EndTurn()
    {
        SetCurrentActorNext();
        StartCoroutine(BeginTurn());
    }
}
