using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SideData : ScriptableObject
{
    public string description;
    public int power;
    public Color backgroundColor;
    public Sprite foregroundImage;

    public abstract bool IsTargetable();
    public abstract void OnUse(Vector2Int target);
    public abstract List<Vector2Int> GetValidTargets(Vector2Int casterPosition);
}
