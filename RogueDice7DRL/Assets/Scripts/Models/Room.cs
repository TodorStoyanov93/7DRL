using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private TileDescriptor[,] _tiles;

    public void SetTiles(TileDescriptor[,] tiles) {
        this._tiles = tiles;
    }

    public TileDescriptor GetTile(Vector2Int vector2Int) {
        if (vector2Int.y >= 0 && vector2Int.y < GetHeight() && vector2Int.x >= 0 && vector2Int.x < GetWidth()) {
            return _tiles[vector2Int.y, vector2Int.x];
        }
        return null;
    }

    public int GetWidth() {
        return _tiles.GetLength(1);
    }

    public int GetHeight()
    {
        return _tiles.GetLength(0);
    }

}
