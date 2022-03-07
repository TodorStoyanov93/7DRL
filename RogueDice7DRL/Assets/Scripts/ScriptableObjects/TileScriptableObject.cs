using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileScriptableObject : ScriptableObject
{
    //public TileTypeEnum tileTypeEnum;

    public Color color;

    public List<GameObject> prefabs;

    public bool isWalkable;
}
