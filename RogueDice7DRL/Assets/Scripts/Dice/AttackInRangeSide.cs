using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackInRangeSide", menuName = "ScriptableObjects/AttackInRangeSide", order = 4)]
public class AttackInRangeSide : SideData
{
    public int range;

    public override void OnUse(Vector2Int target)
    {

        ActionExecutor.Instance.StartCoroutine(OnSideUse(target,power));
    }

    IEnumerator OnSideUse(Vector2Int target,int power) {
        yield return ActionExecutor.Instance.DealDamageInTile(target, power);
    }

    public override List<Vector2Int> GetValidTargets(Vector2Int casterPosition) {

        var tilePositionsToCheck = new List<Vector2Int>();
        for (var x = -range; x <= range; x++) {
            for (var y = -range; y <= range; y++)
            {
                if (x == 0 && y == 0) {
                    continue;
                }
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
