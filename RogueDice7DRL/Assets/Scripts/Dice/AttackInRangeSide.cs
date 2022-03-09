using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackInRangeSide", menuName = "ScriptableObjects/AttackInRangeSide", order = 4)]
public class AttackInRangeSide : SideData
{
    public int range;

    public override void OnUse()
    {
        Debug.Log("Side use");
    }

    public override void OnHover() 
    {
        Debug.Log("Hovered side");
    }


}
