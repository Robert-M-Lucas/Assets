using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VisualUpdate
{
    public int Code;
    public string PieceName;
    public Vector2Int Position;
    public Vector2Int Position2;
    public bool Side;
    public bool Jump;

    private VisualUpdate(int code, string pieceName, Vector2Int position, bool side, Vector2Int position2, bool jump)
    {
        this.Code = code;
        this.PieceName = pieceName;
        this.Position = position;
        this.Position2 = position2;
        this.Side = side;
        this.Jump = jump;
    }

    public static VisualUpdate Create(string pieceName, bool side, Vector2Int position)
    {
        return new VisualUpdate(0, pieceName, position, side, Vector2Int.zero, false);
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