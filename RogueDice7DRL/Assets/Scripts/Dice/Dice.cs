using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "ScriptableObjects/Dice", order = 2)]
public class Dice : ScriptableObject
{
    public string Name;
    public Side sideTop;
    public Side sideBottom;
    public Side sideLeft;
    public Side sideRight;
    public Side sideFront;
    public Side sideBack;
}
