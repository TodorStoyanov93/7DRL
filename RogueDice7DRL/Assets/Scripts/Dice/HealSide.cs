using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "HealSide", menuName = "ScriptableObjects/HealSide", order = 4)]
public class HealSide : SideData
{
    public int range;

    public override void OnUse(Vector2Int target)
    {
        ActionExecutor.Instance.StartCoroutine(OnDiceUse(target));
    }

    IEnumerator OnDiceUse(Vector2Int target)
    {
        yield return ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.ApplyHealthInTile(target, power));
    }

    public override List<Vector2Int> GetValidTargets(Vector2Int casterPosition) {

        var tilePositionsToCheck = new List<Vector2Int>();
        for (var x = -range; x <= range; x++) {
            for (var y = -range; y <= range; y++)
            {
                var tilePos = casterPosition + new Vector2Int(x,y);
                tilePositionsToCheck.Add(tilePos);
            }
        }

        List<Vector2Int> validTiles = tilePositionsToCheck.Where(i =>
        {
            var tile = BoardManager.Instance.room.GetTile(i);
            if (tile == null || !tile.so.isWalkable)
            {
                return false;
            }
            return true;
            
        }).ToList();

        return validTiles;

    }

    public override bool IsTargetable()
    {
        return true;
    }
}
