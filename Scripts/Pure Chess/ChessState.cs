using System.Collections;
using System.Collections.Generic;
using System;
using Random = System.Random;
using UnityEngine;
using ChessPieces;

// Can hold an entire chessboard state
public class ChessState
{
    const int SEED = 73691;
    const int PIECE_HASH_LENGTH = 1;
    const int TURN_HASH_LENGTH = 1;

    public byte[] base_turn_hash = new byte[TURN_HASH_LENGTH];
    public byte[] white_turn_hash = { 0 };
    public byte[] black_turn_hash = { 1 };
    public byte[] hash = new byte[(PIECE_HASH_LENGTH * 64) + (TURN_HASH_LENGTH)];

    public Piece[,] Board;
    public King[] Kings;
    public int move_counter;
    public bool turn;

    public bool real;

    public Dictionary<Vector2Int, List<Tuple<Vector2Int, bool>>> all_possible_moves = new Dictionary<Vector2Int, List<Tuple<Vector2Int, bool>>>();

    public List<VisualUpdate> updates;
    public Tuple<Vector2Int, Vector2Int> last_move;

    // -1 none, 0 black, 1 white
    public int Check = -1;
    public int Checkmate = -1;

    public int PossibleMoves = 0;

    public ChessState(Piece[,] board, King[] kings, int move_counter, bool turn, bool real)
    {
        Board = board;
        Kings = kings;
        this.move_counter = move_counter;
        this.turn = turn;
        this.real = real;
        if (real)
        {
            updates = new List<VisualUpdate>();
            Random random = new Random(SEED);
            random.NextBytes(hash);

            for (int i = PIECE_HASH_LENGTH * 64; i < hash.Length; i ++)
            {
                base_turn_hash[i - PIECE_HASH_LENGTH*64] = hash[i];
            }

        }
        else
        {
            updates = null;
        }
    }

    public void UpdatePossibleMoves()
    {
        PossibleMoves = 0;
        bool checkmate = true;

        all_possible_moves.Clear();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                List<Tuple<Vector2Int, bool>> possible_moves;

                possible_moves = new List<Tuple<Vector2Int, bool>>(); TryGetMoves(new Vector2Int(x, y), out possible_moves);
                if (possible_moves.Count > 0)
                {
                    checkmate = false;
                }

                all_possible_moves[new Vector2Int(x, y)] = possible_moves;
                PossibleMoves += possible_moves.Count;
            }
        }

        if (checkmate)
        {
            if (turn)
            {
                Checkmate = 1;
            }
            else
            {
                Checkmate = 0;
            }
        }
    }

    bool TryGetMoves(Vector2Int position, out List<Tuple<Vector2Int, bool>> possible_moves)
    {
        possible_moves = new List<Tuple<Vector2Int, bool>>();
        Piece piece = GetPieceAtPosition(position);
        if (piece == null || piece.Side != turn) { return false; }

        List<Vector2Int> temp_possible_moves = piece.GetMoves(this);

        foreach (Vector2Int move in temp_possible_moves)
        {
            int check = CheckMoveForCheck(position, move, turn);
            if (check == 1)
            {
                // temp_possible_moves.RemoveAt(i);
            }
            else
            {
                possible_moves.Add(new Tuple<Vector2Int, bool>(move, check == -1));
            }
        }

        return true;

    }

    private int CheckMoveForCheck(Vector2Int position, Vector2Int possible_move, bool side) // 1 = side check or both check, 0 = no check, -1 = !side check
    {
        if (!real) { return 0; }
        int check = 0;

        ChessState new_state = CloneState(false);
        new_state.FullMoveBoardPiece(position, possible_move);
        new_state.EndTurn();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (new_state.Board[x, y] is null) { continue; }
                bool piece_side = new_state.Board[x, y].Side;
                //TODO: TEMP
                if (piece_side == side) { continue; }

                if (!CheckCheckSquares.CanGiveCheck(new_state.Kings[0].Position, new_state.Kings[1].Position, piece_side, new Vector2Int(x, y))) { continue; }

                List<Vector2Int> this_move = new_state.Board[x, y].GetMoves(new_state);
                

                foreach (Vector2Int move in this_move)
                {
                    if (!piece_side && move == new_state.Kings[0].Position)
                    {
                        if (side)
                        {
                            check = 1;
                        }
                        else if (check != 1)
                        {
                            check = -1;
                        }
                    }
                    else if (piece_side && move == new_state.Kings[1].Position)
                    {
                        if (side && check != 1)
                        {
                            check = -1;
                        }
                        else
                        {
                            check = 1;
                        }
                    }
                }
            }
        }

        return check;
    }

    public ChessState CloneState(bool real = false)
    {
        King[] new_kings = new King[2];

        Piece[,] new_board = new Piece[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Board[x, y] is not null)
                {
                    new_board[x, y] = Board[x, y].Clone();
                    if (new_board[x, y].GetType() == typeof(King))
                    {
                        if (new_board[x, y].Side)
                        {
                            new_kings[0] = (King)new_board[x, y];
                        }
                        else
                        {
                            new_kings[1] = (King)new_board[x, y];
                        }
                    }
                }
            }
        }

        return new ChessState(new_board, new_kings, move_counter, turn, real);
    }

    public void CreatePiece(string piece_name, Vector2Int position, bool side)
    {
        Piece piece = PieceFactory.GetPiece(piece_name, position, side);
        Board[position.x, position.y] = piece;
        if (real)
        {
            updates.Add(VisualUpdate.Create(piece_name, side, position));
        }
    }

    public void RemovePiece(Vector2Int position)
    {
        Board[position.x, position.y] = null;
        if (real)
        {
            updates.Add(VisualUpdate.Destroy(position));
        }
    }

    public Piece GetPieceAtPosition(Vector2Int position)
    {
        return Board[position.x, position.y];
    }

    // Doesn't call OnMove
    public void LightMoveBoardPiece(Vector2Int from, Vector2Int to, bool jump = false)
    {
        if (Board[to.x, to.y] is not null && real)
        {
            updates.Add(VisualUpdate.Destroy(to));
        }
        Piece piece = GetPieceAtPosition(from);
        Board[to.x, to.y] = piece;
        piece.Position = to;
        Board[from.x, from.y] = null;
        if (real)
        {
            updates.Add(VisualUpdate.Move(from, to, jump));
        }
        UpdateHash(from, to, piece);
    }

    // Calls OnMove
    public void FullMoveBoardPiece(Vector2Int from, Vector2Int to, bool jump = false)
    {
        LightMoveBoardPiece(from, to, jump);
        Board[to.x, to.y].OnMove(this);
        last_move = new Tuple<Vector2Int, Vector2Int>(from, to);
    }

    public void UpdateHash(Vector2Int from, Vector2Int to, Piece piece)
    {
        byte[] piece_hash = piece.GetHash();

        for (int i = 0; i < piece_hash.Length; i++)
        {
            hash[(from.x*8 + from.y)*PIECE_HASH_LENGTH + i] ^= piece_hash[i];
        }

        for (int i = 0; i < piece_hash.Length; i++)
        {
            hash[(to.x * 8 + to.y) * PIECE_HASH_LENGTH + i] ^= piece_hash[i];
        }
    }

    // Currently unused
    public void UpdateTurnHash()
    {
        if (turn)
        {
            for (int i = 0; i < base_turn_hash.Length; i++)
            {
                hash[i + PIECE_HASH_LENGTH*64] = (byte)(base_turn_hash[i] ^ white_turn_hash[i]);
            }
        }
        else
        {
            for (int i = 0; i < base_turn_hash.Length; i++)
            {
                hash[i + PIECE_HASH_LENGTH * 64] = (byte)(base_turn_hash[i] ^ black_turn_hash[i]);
            }
        }
    }

    public void EndTurn()
    {
        turn = !turn;
        // UpdateTurnHash();
        move_counter++;
    }
}

public static class CheckCheckSquares
{
    public static byte[,] checkCheckSquares = new byte[,]
    {
        { 1,0,0,0,0,0,0,1,0,0,0,0,0,0,1 },
        { 0,1,0,0,0,0,0,1,0,0,0,0,0,1,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,1,0,0 },
        { 0,0,0,1,0,0,0,1,0,0,0,1,0,0,0 },
        { 0,0,0,0,1,0,0,1,0,0,1,0,0,0,0 },
        { 0,0,0,0,0,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,0,1,1,1,1,1,0,0,0,0,0 },
        { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 0,0,0,0,0,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,0,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,0,0,1,0,0,1,0,0,0,0 },
        { 0,0,0,1,0,0,0,1,0,0,0,1,0,0,0 },
        { 0,0,1,0,0,0,0,1,0,0,0,0,1,0,0 },
        { 0,1,0,0,0,0,0,1,0,0,0,0,0,1,0 },
        { 1,0,0,0,0,0,0,1,0,0,0,0,0,0,1 },
    };

    public static Vector2Int center = new Vector2Int(7, 7);

    public static bool CanGiveCheck(Vector2Int WhiteKingPos, Vector2Int BlackKingPos, bool piece_side, Vector2Int piece_pos)
    {
        Vector2Int offset;
        if (!piece_side)
        {
            offset = center - WhiteKingPos;
        }
        else
        {
            offset = center - BlackKingPos;
        }

        Vector2Int lookup_pos = piece_pos + offset;

        return checkCheckSquares[lookup_pos.x, lookup_pos.y] == 1;
    }
}