using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit
{
    public GameObject gameObject;

    public List<Dice> dices;

    public abstract void Turn();
}
