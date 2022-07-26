using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PieceSet
{
    public Piece3D King;
    public Piece3D Queen;
    public Piece3D Rook;
    public Piece3D Bishop;
    public Piece3D Horse;
    public Piece3D Pawn;

    [System.NonSerialized]
    public Dictionary<string, Piece3D> PieceDict;
}

public class Chessboard3DPieceManager : MonoBehaviour
{
    [Header("Setting")]
    public PieceSet Black;
    public PieceSet White;

    [Header("References")]
    public AppearanceManager3D appearanceManager;
    public InputManager inputManager;

    // Internal

    [HideInInspector]
    public List<Piece3D> ActiveWhite = new List<Piece3D>();
    [HideInInspector]
    public List<Piece3D> ActiveBlack = new List<Piece3D>();

    [HideInInspector]
    public Piece3D[,] pieces_on_board = new Piece3D[8, 8];

    public void Awake()
    {
        Black.PieceDict = new Dictionary<string, Piece3D>
        {
            {"King", Black.King },
            {"Queen", Black.Queen },
            {"Rook", Black.Rook },
            {"Bishop", Black.Bishop },
            {"Knight", Black.Horse },
            {"Pawn", Black.Pawn }
        };

        White.PieceDict = new Dictionary<string, Piece3D>
        {
            {"King", White.King },
            {"Queen", White.Queen },
            {"Rook", White.Rook },
            {"Bishop", White.Bishop },
            {"Knight", White.Horse },
            {"Pawn", White.Pawn }
        };
    }

    public void ShowText(bool show)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (pieces_on_board[x, y] is null) { continue; }
                pieces_on_board[x, y].ShowText(show);
            }
        }
    }

    public Vector3 BoardCellToPosition(Vector2Int board_cell)
    {
        return (Vector3.up * appearanceManager.Board[board_cell.x, board_cell.y].transform.localScale.y) / 2;
    }

    public void PlacePiece(string piece_name, bool colour, Vector2Int position)
    {
        PieceSet colourSet;
        Material themeMaterial;
        if (colour) 
        { 
            colourSet = White;
            if ((inputManager.PerspectiveMode && !inputManager.EdgeLit) || appearanceManager.themes[appearanceManager.ActiveTheme].whiteEdgeMaterial is null)
            {
                themeMaterial = appearanceManager.themes[appearanceManager.ActiveTheme].whiteMaterial;
            }
            else
            {
                themeMaterial = appearanceManager.themes[appearanceManager.ActiveTheme].whiteEdgeMaterial;
            }
        }
        else 
        { 
            colourSet = Black; 
            if ((inputManager.PerspectiveMode && !inputManager.EdgeLit) || appearanceManager.themes[appearanceManager.ActiveTheme].whiteEdgeMaterial is null)
            {
                themeMaterial = appearanceManager.themes[appearanceManager.ActiveTheme].blackMaterial;
            }
            else
            {
                themeMaterial = appearanceManager.themes[appearanceManager.ActiveTheme].blackEdgeMaterial;
            }
        }

        Piece3D sourcePiece = colourSet.PieceDict[piece_name];
        GameObject piece = Instantiate(sourcePiece.gameObject);
        foreach (MeshRenderer m in piece.GetComponentsInChildren<MeshRenderer>())
        {
            if (m.gameObject.GetComponent<TMPro.TMP_Text>() is not null) { continue; }
            m.material = themeMaterial;
        }
        piece.transform.SetParent(appearanceManager.Board[position.x, position.y].transform);
        piece.transform.localPosition = BoardCellToPosition(position);

        Piece3D newPiece = piece.GetComponent<Piece3D>();
        newPiece.MovingFrom = piece.transform.position;
        newPiece.TargetPosition = piece.transform.position;
        if (!inputManager.PerspectiveMode) { newPiece.ShowText(true); }

        pieces_on_board[position.x, position.y] = newPiece;
        
    }

    public void RemovePiece(Vector2Int position, bool ripple = true)
    {
        Piece3D piece = pieces_on_board[position.x, position.y];
        DestroyImmediate(piece.gameObject);
        if (ripple)
        {
            appearanceManager.ripples.Add(new RippleData(position));
        }
        pieces_on_board[position.x, position.y] = null;
    }

    public void RemoveAllPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (pieces_on_board[x, y] is not null)
                {
                    RemovePiece(new Vector2Int(x, y), false);
                }
            }
        }
    }

    public void MovePiece(Vector2Int pos1, Vector2Int pos2, bool force_jump = false)
    {
        Piece3D piece = pieces_on_board[pos1.x, pos1.y];

        pieces_on_board[pos1.x, pos1.y] = null;
        pieces_on_board[pos2.x, pos2.y] = piece;
        piece.transform.SetParent(appearanceManager.Board[pos2.x, pos2.y].transform);
        piece.MovingFrom = piece.transform.localPosition;
        piece.TargetPosition = BoardCellToPosition(pos2);
        piece.MoveStartTime = Time.time;
        piece.ForceJump = force_jump;
    }

    public void UpdateTheme()
    {
        ThemePack theme = appearanceManager.themes[appearanceManager.ActiveTheme];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (pieces_on_board[x, y] is null) { continue; }
                
                foreach (MeshRenderer m in pieces_on_board[x, y].gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    if (m.gameObject.GetComponent<TMPro.TMP_Text>() is not null) { continue; }
                    if (pieces_on_board[x, y].PieceColour)
                    {
                        if ((inputManager.PerspectiveMode && !inputManager.EdgeLit) || theme.whiteEdgeMaterial is null)
                        {
                            m.material = theme.whiteMaterial;
                        }
                        else
                        {
                            m.material = theme.whiteEdgeMaterial;
                        }
                    }
                    else
                    {
                        if ((inputManager.PerspectiveMode && !inputManager.EdgeLit) || theme.blackEdgeMaterial is null)
                        {
                            m.material = theme.blackMaterial;
                        }
                        else
                        {
                            m.material = theme.blackEdgeMaterial;
                        }
                    }
                }
            }
        }
    }
}
