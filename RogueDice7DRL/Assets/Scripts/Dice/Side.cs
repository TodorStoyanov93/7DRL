using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Side", menuName = "ScriptableObjects/Side", order = 3)]
public class Side : ScriptableObject
{
    public string description;
    public SideBehavior sideBehavior;
}
