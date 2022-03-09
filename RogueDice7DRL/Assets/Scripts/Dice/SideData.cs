using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SideData", menuName = "ScriptableObjects/SideData", order = 3)]
public abstract class SideData : ScriptableObject
{
    public string description;
    public int power;
    public Sprite backgroundImage;
    public Sprite foregroundImage;
    public abstract void OnUse();
    public abstract void OnHover();
}
