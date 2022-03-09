using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit
{
    public GameObject gameObject;

    public List<ActivatableDice> dices = new List<ActivatableDice>();
    public abstract void Turn();

}
