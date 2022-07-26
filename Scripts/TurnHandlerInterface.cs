using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public abstract class TurnHandlerInterface
{
    public virtual void StartFindingMove(ChessState state) { } 

    public virtual bool MoveAvailable() { return true; }

    public virtual Tuple<Vector2Int, Vector2Int> GetMove()
    {
        return null;
    }

    public virtual float GetMoveFindProgress() { return 0; }

    public virtual void Cleanup() { }
}

public class RandomTurnHandler: TurnHandlerInterface
{
    const float MIN_TIME = 0.2f;

    float _start_time;

    ChessState _state;

    public override void StartFindingMove(ChessState state) 
    {
        _state = state;
        _start_time = Time.time;
    }

    public override bool MoveAvailable()
    {
        return !(Time.time < _start_time + MIN_TIME);
    }

    public override Tuple<Vector2Int, Vector2Int> GetMove()
    {
        var all_pieces_to_move = Enumerable.ToList(_state.all_possible_moves.Keys);

        retry:

        if (all_pieces_to_move.Count == 0) { return null; }

        var to_move = all_pieces_to_move[Random.Range(0, all_pieces_to_move.Count-1)];

        var all_moves = _state.all_possible_moves[to_move];

        if (all_moves.Count == 0)
        {
            all_pieces_to_move.Remove(to_move);
            goto retry;
        }

        var random_move = all_moves[Random.Range(0, all_moves.Count - 1)].Item1;

        return new Tuple<Vector2Int, Vector2Int>(to_move, random_move);
    }
}
