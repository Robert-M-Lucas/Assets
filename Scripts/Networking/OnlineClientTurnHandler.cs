using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading;
using ChessPieces;

public class OnlineClientTurnHandler : TurnHandlerInterface
{
    public bool waiting_for_move = true;

    public Tuple<Vector2Int, Vector2Int> move = null;

    public override void StartFindingMove(ChessState state) 
    { 
        if (state.last_move != null)
        {
            Client.getInstance().SendMessage(ClientSendMovePacket.Build(0, state.last_move.Item1.x, state.last_move.Item1.y, state.last_move.Item2.x, state.last_move.Item2.y));
        }
        waiting_for_move = true;
    }


    public override bool MoveAvailable() { return !waiting_for_move; }

    public override Tuple<Vector2Int, Vector2Int> GetMove()
    {
        return move;
    }

    public override void Cleanup() { Client.getInstance()?.Stop(); Server.getInstance()?.Stop(); }
}