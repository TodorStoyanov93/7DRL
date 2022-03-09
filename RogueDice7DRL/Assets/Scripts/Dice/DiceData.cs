using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceData", menuName = "ScriptableObjects/DiceData", order = 2)]
public class DiceData : ScriptableObject
{
    public string diceName;

    public SideData sideTop;
    public SideData sideBottom;
    public SideData sideLeft;
    public SideData sideRight;
    public SideData sideFront;
    public SideData sideBack;
}
