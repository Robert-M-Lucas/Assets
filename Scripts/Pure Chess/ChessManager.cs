using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ChessPieces;

public class ChessManager : MonoBehaviour
{
    public Chessboard3DPieceManager pieceManager3D;
    public CanvasManager canvasManager;

    public ChessState State;

    public void ApplyVisualUpdate(List<VisualUpdate> updates)
    {
        foreach (VisualUpdate update in updates)
        {
            ApplyVisualUpdate(update);
        }
    }

    public void ApplyVisualUpdate(VisualUpdate update)
    {
        if (update.Code == 0)
        {
            pieceManager3D.PlacePiece(update.PieceName, update.Side, update.Position);
        }
        else if (update.Code == 1)
        {
            pieceManager3D.RemovePiece(update.Position);
        }
        else
        {
            pieceManager3D.MovePiece(update.Position, update.Position2, update.Jump);
        }
    }
    
    public void InitialiseBoard()
    {
        pieceManager3D.RemoveAllPieces();
        State = new ChessState(new Piece[8, 8], new King[2], 0, true, true);

        State.CreatePiece("King", new Vector2Int(4, 0), true);
        State.Kings[0] = (King)State.GetPieceAtPosition(new Vector2Int(4, 0));
        State.CreatePiece("Queen", new Vector2Int(3, 0), true);
        State.CreatePiece("Bishop", new Vector2Int(2, 0), true);
        State.CreatePiece("Bishop", new Vector2Int(5, 0), true);
        State.CreatePiece("Rook", new Vector2Int(0, 0), true);
        State.CreatePiece("Rook", new Vector2Int(7, 0), true);
        State.CreatePiece("Knight", new Vector2Int(1, 0), true);
        State.CreatePiece("Knight", new Vector2Int(6, 0), true);

        for (int x = 0; x < 8; x++) { State.CreatePiece("Pawn", new Vector2Int(x, 1), true); }

        State.CreatePiece("King", new Vector2Int(4, 7), false);  
        State.Kings[1] = (King)State.GetPieceAtPosition(new Vector2Int(4, 7));

        State.CreatePiece("Queen", new Vector2Int(3, 7), false);
        State.CreatePiece("Bishop", new Vector2Int(2, 7), false);
        State.CreatePiece("Bishop", new Vector2Int(5, 7), false);
        State.CreatePiece("Rook", new Vector2Int(0, 7), false);
        State.CreatePiece("Rook", new Vector2Int(7, 7), false);
        State.CreatePiece("Knight", new Vector2Int(1, 7), false);
        State.CreatePiece("Knight", new Vector2Int(6, 7), false);
        
        for (int x = 0; x < 8; x++) { State.CreatePiece("Pawn", new Vector2Int(x, 6), false); }

        State.UpdatePossibleMoves();
        ApplyVisualUpdate(State.updates);
        State.updates.Clear();
    }

    /*
    private Piece[,] CloneBoard(Piece[,] board_in, out King[] kings_to_use)
    {
        kings_to_use = new King[2];

        Piece[,] out_board = new Piece[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board_in[x, y] is not null)
                {
                    out_board[x, y] = board_in[x, y].Clone();
                    if (out_board[x, y].GetType() == typeof(King))
                    {
                        if (out_board[x, y].side)
                        {
                            kings_to_use[0] = (King)out_board[x, y];
                        }
                        else
                        {
                            kings_to_use[1] = (King)out_board[x, y];
                        }
                    }
                }
            }
        }

        return out_board;
    }
    */

    public void TryMovePiece(Vector2Int pos1, Vector2Int pos2)
    {
        State.FullMoveBoardPiece(pos1, pos2);
        State.EndTurn();
        State.UpdatePossibleMoves();
        ApplyVisualUpdate(State.updates);
        State.updates.Clear();

        if (State.Checkmate != -1)
        {
            canvasManager.ShowCheckmate();
        }
    }

    public List<Tuple<Vector2Int, bool>> GetMovesFromAllMoves(Vector2Int position)
    {
        return new List<Tuple<Vector2Int, bool>>(State.all_possible_moves[position]);
    }
}
