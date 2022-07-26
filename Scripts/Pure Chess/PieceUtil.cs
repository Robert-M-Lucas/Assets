using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessPieces
{
    public static class PieceFactory
    {
        public static Piece GetPiece(string pieceString, Vector2Int initPosition, bool side)
        {
            switch (pieceString)
            {
                case "King":
                    return new King(initPosition, side);
                case "Queen":
                    return new Queen(initPosition, side);
                case "Bishop":
                    return new Bishop(initPosition, side);
                case "Rook":
                    return new Rook(initPosition, side);
                case "Knight":
                    return new Knight(initPosition, side);
                case "Pawn":
                    return new Pawn(initPosition, side);
                default:
                    throw new ArgumentException($"Invalid piece: {pieceString}");
            }
        }
    }

    public abstract class Piece
    {
        public Vector2Int Position;
        public bool Side;

        public Piece(Vector2Int initPosition, bool side)
        {
            Position = initPosition;
            this.Side = side;
        }

        public abstract float GetValue();

        public abstract byte[] GetHash();

        public abstract List<Vector2Int> GetMoves(ChessState state);

        public virtual void OnMove(ChessState state) { }

        public abstract Piece Clone();
    }

    public static class PieceUtil
    {
        public static bool IsInBounds(Vector2Int position) { return !(position.x < 0 || position.y < 0 || position.x > 7 || position.y > 7); }

        public static List<Vector2Int> CullOutOfBoundsMoves(List<Vector2Int> moves)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if (!IsInBounds(moves[i])) { moves.RemoveAt(i); }
            }

            return moves;
        }

        public static List<Vector2Int> RaycastMoves(Vector2Int step, Vector2Int position, ChessState state, int maxRepeats, bool side, bool attacking = true, bool exclusiveAttack = false)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            for (int i = 0; i < maxRepeats; i++)
            {
                position += step;

                if (!IsInBounds(position)) { break; }

                Piece piece_at_pos = state.GetPieceAtPosition(position);
                if (piece_at_pos is not null)
                {
                    if (piece_at_pos.Side == side) { break; }
                    else
                    {
                        if (attacking) { moves.Add(position); break; }
                        break;
                    }
                }
                else if (exclusiveAttack)
                {
                    break;
                }
                moves.Add(position);
            }

            return moves;
        }
    }

}