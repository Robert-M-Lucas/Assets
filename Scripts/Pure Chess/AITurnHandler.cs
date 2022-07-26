using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading;
using ChessPieces;

public class AITurnHandler : TurnHandlerInterface
{
    const int DEPTH = 4;
    const float HEATMAP_MULTIPLIER = 0.25f;
    const float POSSIBLE_MOVES_MULTIPLIER = 0.15f;

    bool finding_move = false;
    float progress = 0;
    Thread MoveFindThread = null;
    ChessState chessState;

    public Tuple<Vector2Int, Vector2Int> move;

    public override void StartFindingMove(ChessState state)
    {
        if (finding_move) { throw new Exception("Alreading finding move"); }
        finding_move = true;
        progress = 0;
        chessState = state;
        MoveFindThread = new Thread(FindMove);
        MoveFindThread.Start();
    }

    void FindMove()
    {
        float curr_max = float.MinValue;
        Tuple<Vector2Int, Vector2Int> best_move = null;

        foreach (var move in chessState.all_possible_moves)
        {
            foreach (var move_to in move.Value)
            {
                progress += 1 / (float)chessState.PossibleMoves;
                ChessState new_state = chessState.CloneState();
                new_state.FullMoveBoardPiece(move.Key, move_to.Item1);
                new_state.EndTurn();
                new_state.UpdatePossibleMoves();

                float score = RecursivelyGetMoveScore(new_state, 0, false, curr_max);
                if (score > curr_max)
                {
                    curr_max = score;
                    best_move = new Tuple<Vector2Int, Vector2Int>(move.Key, move_to.Item1);
                }
            }
        }

        if (best_move is null) { throw new Exception("NO MOVE AVAILABLE"); }
        move = best_move;
        progress = 1;
        finding_move = false;
    }

    float GetScoreOfState(ChessState state)
    {
        if ((state.Checkmate == 0 && !chessState.turn) || (state.Checkmate == 1 && chessState.turn))
        {
            return -1000;
        }
        else if ((state.Checkmate == 1 && !chessState.turn) || (state.Checkmate == 0 && chessState.turn)) 
        {
            return 1000;
        }

        float score = 0;

        if (state.turn)
        {
            score += state.PossibleMoves * POSSIBLE_MOVES_MULTIPLIER;
        }
        else
        {
            score -= state.PossibleMoves * POSSIBLE_MOVES_MULTIPLIER;
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (state.Board[x, y] is null) { continue; }

                Piece piece = state.Board[x, y];

                float[,] heatmap;
                if (!PieceHeatmaps.TryGetValue(piece.GetType(), out heatmap))
                {
                    heatmap = new float[8, 8];
                }

                if (state.Board[x,y].Side == chessState.turn)
                {
                    score += state.Board[x, y].GetValue();
                    if (chessState.turn)
                    {
                        score += heatmap[y, x] * HEATMAP_MULTIPLIER;
                        
                    }
                    else
                    {
                        score += heatmap[x, 7-y] * HEATMAP_MULTIPLIER;
                    }
                }
                else
                {
                    score -= state.Board[x, y].GetValue();

                    if (chessState.turn)
                    {
                        score -= heatmap[y, x] * HEATMAP_MULTIPLIER;
                    }
                    else
                    {
                        score -= heatmap[7 - y, x] * HEATMAP_MULTIPLIER;
                    }
                }
            }
        }

        return score;
    }

    float RecursivelyGetMoveScore(ChessState false_state, int depth, bool? min_or_max = null, float prune = 0)
    {
        if (depth >= DEPTH)
        {
            return GetScoreOfState(false_state);
        }
        else
        {
            if (false_state.PossibleMoves == 0 && false_state.turn != chessState.turn) { return 100; }
            else if (false_state.PossibleMoves == 0 && false_state.turn == chessState.turn) { return -100; }

            float curr_min = float.MaxValue;
            float curr_max = float.MinValue;

            foreach (var move in false_state.all_possible_moves)
            {
                foreach (var move_to in move.Value)
                {
                    ChessState new_state = false_state.CloneState();
                    new_state.FullMoveBoardPiece(move.Key, move_to.Item1);
                    new_state.EndTurn();

                    if (depth < DEPTH-1)
                    {
                        new_state.UpdatePossibleMoves();
                    }

                    float score;

                    if (false_state.turn != chessState.turn)
                    {
                        score = RecursivelyGetMoveScore(new_state, depth + 1, true, curr_min);
                    }
                    else
                    {
                        score = RecursivelyGetMoveScore(new_state, depth + 1, false, curr_max);
                    }
                    

                    if (min_or_max != null)
                    {
                        if ((score > prune && (bool)min_or_max) || (score < prune && !(bool)min_or_max))
                        {
                            return score;
                        }
                    }

                    if (score < curr_min) { curr_min = score; }
                    if (score > curr_max) { curr_max = score; }
                }
            }


            if (false_state.turn == chessState.turn) { return curr_max; }
            else { return curr_min; }
        }
    }

    /* NOT MINIMAX
    float RecursivelyGetMoveScore(ChessState false_state, int depth, float weight, float previous_score)
    {
        float board_score = GetScoreOfState(false_state);

        if (depth >= DEPTH)
        {
            return board_score * weight;
        }
        else
        {
            // Assume opponent will play moves that give them more points
            if (board_score < previous_score)
            {
                weight *= 1 + ((previous_score - board_score) * 2);
            }

            float score = 0;
            int number_of_moves = 0;

            foreach (var move in false_state.all_possible_moves)
            {
                foreach (var move_to in move.Value)
                {
                    ChessState new_state = false_state.CloneState();
                    new_state.FullMoveBoardPiece(move.Key, move_to.Item1);
                    new_state.EndTurn();

                    if (depth < DEPTH-1)
                    {
                        new_state.UpdatePossibleMoves();
                    }

                    score += RecursivelyGetMoveScore(new_state, depth + 1, weight, board_score);
                    number_of_moves++;
                }
            }

            if (number_of_moves == 0 && false_state.turn != chessState.turn) { return 100; }
            else if (number_of_moves == 0 && false_state.turn == chessState.turn) { return -100; }

            return score / number_of_moves;
        }
    }
    */

    public override bool MoveAvailable()
    {
        return !finding_move;
    }

    public override float GetMoveFindProgress()
    {
        return progress;
    }

    public override Tuple<Vector2Int, Vector2Int> GetMove()
    {
        return move;
    }

    public override void Cleanup()
    {
        if (MoveFindThread != null)
        {
            MoveFindThread.Abort();
        }
    }

    Dictionary<Type, float[,]> PieceHeatmaps = new Dictionary<Type, float[,]>
    {
        { 
            typeof(King),
            new float[,] {
                { 1    ,0.9f ,0.8f ,0.7f ,0.7f ,0.8f ,0.9f ,1    },
                { 0.5f ,0.4f ,0.3f ,0.2f ,0.2f ,0.3f ,0.4f ,0.5f },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
        }
        },
        {
            typeof(Rook),
            new float[,] {
                { 0    ,0    ,0.2f ,0.5f ,0.5f ,0.2f ,0    ,0    },
                { 0    ,0    ,0    ,0.5f ,0.5f ,0    ,0    ,0    },
                { 0.2f ,0.2f ,0.4f ,0.5f ,0.5f ,0.4f ,0.2f ,0.2f },
                { 0.2f ,0.2f ,0.4f ,0.5f ,0.5f ,0.4f ,0.2f ,0.2f },
                { 0.2f ,0.2f ,0.3f ,0.6f ,0.6f ,0.3f ,0.2f ,0.2f },
                { 0.2f ,0.2f ,0.3f ,0.6f ,0.6f ,0.3f ,0.2f ,0.2f },
                { 0.2f ,0.3f ,0.3f ,0.6f ,0.6f ,0.3f ,0.2f ,0.2f },
                { 0.3f ,0.5f ,0.5f ,0.6f ,0.6f ,0.5f ,0.5f ,0.3f },
        }
        },
        {
            typeof(Pawn),
            new float[,] {
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0    ,0    ,0    ,0    ,0    ,0    ,0    ,0    },
                { 0.2f ,0.2f ,0.3f ,0.5f ,0.5f ,0.3f ,0.2f ,0.2f },
                { 0.4f ,0.3f ,0.3f ,0.6f ,0.6f ,0.3f ,0.3f ,0.4f },
                { 0.5f ,0.4f ,0.4f ,0.7f ,0.7f ,0.4f ,0.4f ,0.5f },
                { 0.8f ,0.8f ,0.8f ,0.8f ,0.8f ,0.8f ,0.8f ,0.8f },
                { 0.9f ,0.9f ,0.9f ,0.9f ,0.9f ,0.9f ,0.9f ,0.9f },
                { 1    ,1    ,1    ,1    ,1    ,1    ,1    ,1    },
        }
        },
    };
}
