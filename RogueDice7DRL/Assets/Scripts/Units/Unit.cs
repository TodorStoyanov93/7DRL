using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit
{
    public GameObject gameObject;

    public int maxHealth = 10;
    public int currentHealth = 2;
    public int shield = 0;

    public List<ActivatableDice> dices = new List<ActivatableDice>();
    public abstract void Turn();

    public Vector2Int GetVector2IntPosition() {
        return Helpers.RoundToVector2Int(gameObject.transform.position);
    }
}
