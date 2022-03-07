using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Actor> actors;
    public Actor currentActor;
    private int currentActorIndex;
    public static TurnSystem Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        actors = new List<Actor>();
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
        BeginTurn();

    }

    private void SetCurrentActorFirst()
    {
        currentActorIndex = 0;
        currentActor = actors[currentActorIndex];
    }

    private void SetCurrentActorNext() {
        currentActorIndex++;
        currentActorIndex %= actors.Count;
    }


    private void EnsureAtleast1Actor()
    {
        if (actors.Count == 0) {
            Debug.Log("No actors in scene");
        } 
    }

    void BeginTurn() {
        //CameraController.Instance.SnapTo(currentActor.unit.gameObject);
    }
}
