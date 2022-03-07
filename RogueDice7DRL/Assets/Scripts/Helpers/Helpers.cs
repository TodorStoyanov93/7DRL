using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers : MonoBehaviour
{
    public Vector2Int RoundToVector2Int(Vector2 vector2) {
        return new Vector2Int(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
    }
}
