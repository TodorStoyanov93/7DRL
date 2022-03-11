using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BlastSide", menuName = "ScriptableObjects/BlastSide", order = 4)]
public class BlastSide : SideData
{
    public int range;

    public override void OnUse(Vector2Int target)
    {
        ActionExecutor.Instance.StartCoroutine(OnDiceUse(target));
    }

    IEnumerator OnDiceUse(Vector2Int target) {
        var x1 =  ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.DealDamageInTile(target, power));
        yield return new WaitForSeconds(0.05f);
        var x2 = ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.DealDamageInTile(target + new Vector2Int(1, 0), power));
        yield return new WaitForSeconds(0.05f);
        var x3 = ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.DealDamageInTile(target + new Vector2Int(-1, 0), power));
        yield return new WaitForSeconds(0.05f);
        var x4 = ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.DealDamageInTile(target + new Vector2Int(0, 1), power));
        yield return new WaitForSeconds(0.05f);
        var x5 = ActionExecutor.Instance.StartCoroutine(ActionExecutor.Instance.DealDamageInTile(target + new Vector2Int(0, -1), power));

        yield return x1;
        yield return x2;
        yield return x3;
        yield return x4;
        yield return x5;
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
