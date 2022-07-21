using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VisualUpdate
{
    public int code;
    public string piece_name;
    public Vector2Int position;
    public Vector2Int position2;
    public bool side;
    public bool jump;

    private VisualUpdate(int code, string piece_name, Vector2Int position, bool side, Vector2Int position2, bool jump)
    {
        this.code = code;
        this.piece_name = piece_name;
        this.position = position;
        this.position2 = position2;
        this.side = side;
        this.jump = jump;
    }

    public static VisualUpdate Create(string piece_name, bool side, Vector2Int position)
    {
        return new VisualUpdate(0, piece_name, position, side, Vector2Int.zero, false);
    }

    public static VisualUpdate Destroy(Vector2Int position)
    {
        return new VisualUpdate(1, "", position, true, Vector2Int.zero, false);
    }

    public static VisualUpdate Move(Vector2Int position, Vector2Int position2, bool jump)
    {
        return new VisualUpdate(2, "", position, true, position2, jump);
    }
}