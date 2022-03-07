using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public List<Room> teleportRooms;

    private TileDescriptor[,] _tiles;

    public void SetTiles(TileDescriptor[,] tiles) {
        this._tiles = tiles;
    }

    public TileDescriptor[,] GetTiles() {
        return _tiles;
    }

    public TileDescriptor GetTile(Vector2Int vector2Int) {
        return _tiles[vector2Int.y, vector2Int.x];
    }

    public TileDescriptor GetTile(int x, int y)
    {
        return _tiles[y, x];
    }

    public int GetWidth() {
        return _tiles.GetLength(1);
    }

    public int GetHeight()
    {
        return _tiles.GetLength(0);
    }

}
