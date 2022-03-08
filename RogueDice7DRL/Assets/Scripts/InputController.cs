using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public static InputController Instance { get; private set; }
    private bool isActive;
    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && playerControlledUnit != null) {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Ground");

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var didRaycastHit = Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 0.1f);

            if (didRaycastHit) {
                Vector2Int hitGoPos = new Vector2Int(Mathf.RoundToInt(hit.collider.gameObject.transform.position.x),
                    Mathf.RoundToInt(hit.collider.gameObject.transform.position.y));
                if (Input.GetMouseButtonDown(0)) //not if over ui element
                {
                    if (isActive) { 
                        playerControlledUnit.OnTileClick(hitGoPos);
                    }
                }
                if (isActive) { 
                    playerControlledUnit.OnTileHover(hitGoPos);
                }

            }

            if (Input.GetMouseButtonDown(1)) {
                if (isActive) { 
                    playerControlledUnit.OnRightClick();
                }
            }
        }
    }


    private PlayerControlledUnit playerControlledUnit;
    internal void WaitForInput(PlayerControlledUnit playerControlledUnit)
    {
        this.playerControlledUnit = playerControlledUnit;
        isActive = true;
    }

    internal void StopWaitingForInput() {
        isActive = false;
    }
}
