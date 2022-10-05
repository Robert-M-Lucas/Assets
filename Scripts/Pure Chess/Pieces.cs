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
        int _move_count;

        public King(Vector2Int initPosition, bool side) : base(initPosition, side) { }

        public override float GetValue() => 90f;

        public override byte[] GetHash() => new byte[] { 0 };

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            int rook_y = 7;
            if (Side) { rook_y = 0; }

            List<Vector2Int> moves =
                PieceUtil.RaycastMoves(new Vector2Int(1, 0), Position, state, 1, Side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), Position, state, 1, Side, true));

            if (_move_count == 0)
            {
                Piece left_piece = state.GetPieceAtPosition(new Vector2Int(0, rook_y));
                if (left_piece is not null && left_piece.GetType() == typeof(Rook) && ((Rook)left_piece).MoveCount == 0)
                {
                    var temp_moves = PieceUtil.RaycastMoves(new Vector2Int(-1, 0), Position, state, 3, Side, false);
                    if (temp_moves.Count > 2)
                    {
                        moves.Add(temp_moves[1]);
                    }
                }

                Piece right_piece = state.GetPieceAtPosition(new Vector2Int(7, rook_y));
                if (right_piece is not null && right_piece.GetType() == typeof(Rook) && ((Rook)right_piece).MoveCount == 0)
                {
                    var temp_moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), Position, state, 2, Side, false);
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
            if (Side) { rook_y = 0; }

            if (_move_count == 0)
            {
                if (Position.x == 2)
                {
                    state.LightMoveBoardPiece(new Vector2Int(0, rook_y), Position + new Vector2Int(1, 0), true);
                }
                else if (Position.x == 6)
                {
                    state.LightMoveBoardPiece(new Vector2Int(7, rook_y), Position - new Vector2Int(1, 0), true);
                }
            }

            _move_count++;
        }

        public override Piece Clone()
        {
            King new_king = new King(Position, Side);
            new_king._move_count = _move_count;
            return new_king;
        }
    }

    public class Queen : Piece
    {
        public override float GetValue() => 9f;

        public override byte[] GetHash() => new byte[] { 1 };

        public Queen(Vector2Int initPosition, bool side) : base(initPosition, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), Position, state, 10, Side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), Position, state, 10, Side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Queen new_queen = new Queen(Position, Side);
            return new_queen;
        }
    }

    public class Bishop : Piece
    {
        public override float GetValue() => 3f;

        public override byte[] GetHash() => new byte[] { 2 };
        public Bishop(Vector2Int initPosition, bool side) : base(initPosition, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 1), Position, state, 10, Side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(ChessManagerInterface.UNSELECTED, Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -1), Position, state, 10, Side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Bishop new_bishop = new Bishop(Position, Side);
            return new_bishop;
        }
    }

    public class Rook : Piece
    {
        public override float GetValue() => 5f;

        public override byte[] GetHash() => new byte[] { 3 };

        public int MoveCount;
        public Rook(Vector2Int init_position, bool side) : base(init_position, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(1, 0), Position, state, 10, Side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 0), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, -1), Position, state, 10, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(0, 1), Position, state, 10, Side, true));

            return moves;
        }

        public override void OnMove(ChessState state) { MoveCount++; }

        public override Piece Clone()
        {
            Rook new_rook = new Rook(Position, Side);
            new_rook.MoveCount = MoveCount;
            return new_rook;
        }
    }

    public class Pawn : Piece
    {
        public override float GetValue() => 1f;

        public override byte[] GetHash() => new byte[] { 4 };

        int _move_count;
        int _last_move = -1;

        public Pawn(Vector2Int initPosition, bool side) : base(initPosition, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            int modifier = -1;
            if (Side) { modifier = 1; }

            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(0, 1) * modifier, Position, state, 1, Side, false);
            if (_move_count == 0)
            {
                List<Vector2Int> dash = PieceUtil.RaycastMoves(new Vector2Int(0, 1) * modifier, Position, state, 2, Side, false);
                if (dash.Count > 1)
                {
                    moves.Add(dash[1]);
                }
            }

            int pawn_rank = 3;
            int forward = -1;
            if (Side) { pawn_rank = 4; forward = 1; }

            if (PieceUtil.IsInBounds(Position + Vector2Int.left))
            {
                Piece piece = state.GetPieceAtPosition(Position + Vector2Int.left);
                if (piece != null && piece.Side != Side && piece.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)piece;
                    if (pawn._move_count == 1 && pawn._last_move == state.move_counter - 1 && Position.y == pawn_rank)
                    {
                        moves.Add(Position + new Vector2Int(-1, forward));
                    }
                }
            }

            if (PieceUtil.IsInBounds(Position + Vector2Int.right))
            {
                Piece piece = state.GetPieceAtPosition(Position + Vector2Int.right);
                if (piece != null && piece.Side != Side && piece.GetType() == typeof(Pawn))
                {
                    Pawn pawn = (Pawn)piece;
                    if (pawn._move_count == 1 && pawn._last_move == state.move_counter - 1 && Position.y == pawn_rank)
                    {
                        moves.Add(Position + new Vector2Int(1, forward));
                    }
                }
            }

            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 1) * modifier, Position, state, 1, Side, true, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 1) * modifier, Position, state, 1, Side, true, true));

            return moves;
        }

        public override void OnMove(ChessState state)
        {
            _move_count++;
            _last_move = state.move_counter;

            int friend_rank = 2;
            int pawn_rank = 3;
            if (Side) { pawn_rank = 4; friend_rank = 5; }

            Piece piece = state.GetPieceAtPosition(new Vector2Int(Position.x, pawn_rank));
            if (Position.y == friend_rank && piece is not null && piece.GetType() == typeof(Pawn))
            {
                Pawn pawn = (Pawn)piece;
                if (pawn._move_count == 1 && pawn._last_move == state.move_counter - 1)
                {
                    state.RemovePiece(new Vector2Int(Position.x, pawn_rank));
                }
            }

            if ((Position.y == 7 && Side) || (Position.y == 0 && !Side))
            {
                state.RemovePiece(Position);
                state.CreatePiece("Queen", Position, Side);
            }
        }

        public override Piece Clone()
        {
            Pawn new_pawn = new Pawn(Position, Side);
            new_pawn._move_count = _move_count;
            new_pawn._last_move = _last_move;
            return new_pawn;
        }
    }

    public class Knight : Piece
    {
        public override float GetValue() => 3f;

        public override byte[] GetHash() => new byte[] { 5 };

        public Knight(Vector2Int initPosition, bool side) : base(initPosition, side)
        {
        }

        public override List<Vector2Int> GetMoves(ChessState state)
        {
            List<Vector2Int> moves = PieceUtil.RaycastMoves(new Vector2Int(2, 1), Position, state, 1, Side, true);
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(2, -1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, 2), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(1, -2), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, -2), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-1, 2), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-2, 1), Position, state, 1, Side, true));
            moves.AddRange(PieceUtil.RaycastMoves(new Vector2Int(-2, -1), Position, state, 1, Side, true));

            return moves;
        }

        public override Piece Clone()
        {
            Knight new_horse = new Knight(Position, Side);
            return new_horse;
        }
    }
}