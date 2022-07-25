using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessPieces
{
    public class King : Piece
    {
        int move_count;

        public King(Vector2Int init_position, bool side) : base(init_position, side) { }

        public override float GetValue() => 90f;

        public override byte[] GetHash() => new byte[] { 0 };

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            int rook_y = 7;
            if (side) { rook_y = 0; }

            List<Vector2Int> moves =
                PieceUtil.RaycastMoves(new Vector2Int(1, 0), position, state, 1, side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), position, state, 1, side, true));

            if (move_count == 0)
            {
                Piece left_piece = state.GetPieceAtPosition(new Vector2Int(0, rook_y));
                if (left_piece is not null && left_piece.GetType() == typeof(Rook) && ((Rook)left_piece).move_count == 0)
                {
                    var temp_moves = PieceUtil.RaycastMoves(new Vector2Int(-1, 0), position, state, 3, side, false);
                    if (temp_moves.Count > 2)
                    {
                        moves.Add(temp_moves[1]);
                    }
                }

                Piece right_piece = state.GetPieceAtPosition(new Vector2Int(7, rook_y));
                if (right_piece is not null && right_piece.GetType() == typeof(Rook) && ((Rook)right_piece).move_count == 0)
                {
                    var temp_moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), position, state, 2, side, false);
                    if (temp_moves.Count > 1)
                    {
                        moves.Add(temp_moves[1]);
                    }
                }
            }

            return moves;
        }

        public override void OnMove(ChessState state)
        {
            int rook_y = 7;
            if (side) { rook_y = 0; }

            if (move_count == 0)
            {
                if (position.x == 2)
                {
                    state.LightMoveBoardPiece(new Vector2Int(0, rook_y), position + new Vector2Int(1, 0), true);
                }
                else if (position.x == 6)
                {
                    state.LightMoveBoardPiece(new Vector2Int(7, rook_y), position - new Vector2Int(1, 0), true);
                }
            }

            move_count++;
        }

        public override Piece Clone()
        {
            King new_king = new King(position, side);
            new_king.move_count = move_count;
            return new_king;
        }
    }

    public class Queen : Piece
    {
        public override float GetValue() => 10f;

        public override byte[] GetHash() => new byte[] { 1 };

        public Queen(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), position, state, 10, side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), position, state, 10, side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Queen new_queen = new Queen(position, side);
            return new_queen;
        }
    }

    public class Bishop : Piece
    {
        public override float GetValue() => 3f;

        public override byte[] GetHash() => new byte[] { 2 };
        public Bishop(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 1), position, state, 10, side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), position, state, 10, side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Bishop new_bishop = new Bishop(position, side);
            return new_bishop;
        }
    }

    public class Rook : Piece
    {
        public override float GetValue() => 5f;

        public override byte[] GetHash() => new byte[] { 3 };

        public int move_count;
        public Rook(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), position, state, 10, side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), position, state, 10, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), position, state, 10, side, true));

            return moves;
        }

        public override void OnMove(ChessState state) { move_count++; }

        public override Piece Clone()
        {
            Rook new_rook = new Rook(position, side);
            new_rook.move_count = move_count;
            return new_rook;
        }
    }

    public class Pawn : Piece
    {
        public override float GetValue() => 1f;

        public override byte[] GetHash() => new byte[] { 4 };

        int move_count;
        int last_move = -1;

        public Pawn(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            int modifier = -1;
            if (side) { modifier = 1; }

            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(0, 1) * modifier, position, state, 1, side, false);
            if (move_count == 0)
            {
                List<Vector2Int> dash = PieceUtil.RaycastMoves(new Vector2Int(0, 1) * modifier, position, state, 2, side, false);
                if (dash.Count > 1)
                {
                    moves.Add(dash[1]);
                }
            }

            int pawn_rank = 3;
            int forward = -1;
            if (side) { pawn_rank = 4; forward = 1; }

            if (PieceUtil.IsInBounds(position + Vector2Int.left))
            {
                Piece piece = state.GetPieceAtPosition(position + Vector2Int.left);
                if (piece != null && piece.side != side && piece.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)piece;
                    if (pawn.move_count == 1 && pawn.last_move == state.move_counter - 1 && position.y == pawn_rank)
                    {
                        moves.Add(position + new Vector2Int(-1, forward));
                    }
                }
            }

            if (PieceUtil.IsInBounds(position + Vector2Int.right))
            {
                Piece piece = state.GetPieceAtPosition(position + Vector2Int.right);
                if (piece != null && piece.side != side && piece.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)piece;
                    if (pawn.move_count == 1 && pawn.last_move == state.move_counter - 1 && position.y == pawn_rank)
                    {
                        moves.Add(position + new Vector2Int(1, forward));
                    }
                }
            }

            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1) * modifier, position, state, 1, side, true, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1) * modifier, position, state, 1, side, true, true));

            return moves;
        }

        public override void OnMove(ChessState state)
        {
            move_count++;
            last_move = state.move_counter;

            int friend_rank = 2;
            int pawn_rank = 3;
            if (side) { pawn_rank = 4; friend_rank = 5; }

            Piece piece = state.GetPieceAtPosition(new Vector2Int(position.x, pawn_rank));
            if (position.y == friend_rank && piece is not null && piece.GetType() == typeof(Pawn))
            {
                Pawn pawn = (Pawn)piece;
                if (pawn.move_count == 1 && pawn.last_move == state.move_counter - 1)
                {
                    state.RemovePiece(new Vector2Int(position.x, pawn_rank));
                }
            }

            if ((position.y == 7 && side) || (position.y == 0 && !side))
            {
                state.RemovePiece(position);
                state.CreatePiece("Queen", position, side);
            }
        }

        public override Piece Clone()
        {
            Pawn new_pawn = new Pawn(position, side);
            new_pawn.move_count = move_count;
            new_pawn.last_move = last_move;
            return new_pawn;
        }
    }

    public class Knight : Piece
    {
        public override float GetValue() => 3f;

        public override byte[] GetHash() => new byte[] { 5 };

        public Knight(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(2, 1), position, state, 1, side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(2, -1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 2), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -2), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, -2), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 2), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-2, 1), position, state, 1, side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-2, -1), position, state, 1, side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Knight new_horse = new Knight(position, side);
            return new_horse;
        }
    }
}