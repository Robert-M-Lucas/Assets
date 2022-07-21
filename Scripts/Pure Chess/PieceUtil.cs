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
        public static Piece GetPiece(string piece_string, Vector2Int init_position, bool side)
        {
            switch (piece_string)
            {
                case "King":
                    return new King(init_position, side);
                case "Queen":
                    return new Queen(init_position, side);
                case "Bishop":
                    return new Bishop(init_position, side);
                case "Rook":
                    return new Rook(init_position, side);
                case "Knight":
                    return new Knight(init_position, side);
                case "Pawn":
                    return new Pawn(init_position, side);
                default:
                    throw new ArgumentException($"Invalid piece: {piece_string}");
            }
        }
    }

    public abstract class Piece
    {
        public Vector2Int position;
        public bool side;

        public Piece(Vector2Int init_position, bool side)
        {
            position = init_position;
            this.side = side;
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

        public static List<Vector2Int> RaycastMoves(Vector2Int step, Vector2Int position, ChessState state, int max_repeats, bool side, bool attacking = true, bool exclusive_attack = false)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            for (int i = 0; i < max_repeats; i++)
            {
                position += step;

                if (!IsInBounds(position)) { break; }

                Piece piece_at_pos = state.GetPieceAtPosition(position);
                if (piece_at_pos is not null)
                {
                    if (piece_at_pos.side == side) { break; }
                    else
                    {
                        if (attacking) { moves.Add(position); break; }
                        break;
                    }
                }
                else if (exclusive_attack)
                {
                    break;
                }
                moves.Add(position);
            }

            return moves;
        }
    }

}