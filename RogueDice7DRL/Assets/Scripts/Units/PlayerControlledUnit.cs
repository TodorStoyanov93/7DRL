using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledUnit : Unit
{
    public override void Turn()
    {
        InputHandler.Instance.TurnBeginsForPlayer();
    }
    
}
