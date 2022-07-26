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
    public bool WaitingForMove = true;

    public Tuple<Vector2Int, Vector2Int> Move = null;

    public override void StartFindingMove(ChessState state) 
    { 
        if (state.last_move != null)
        {
            Client.getInstance().SendMessage(ClientSendMovePacket.Build(0, state.last_move.Item1.x, state.last_move.Item1.y, state.last_move.Item2.x, state.last_move.Item2.y));
        }
        WaitingForMove = true;
    }


    public override bool MoveAvailable() { return !WaitingForMove; }

    public override Tuple<Vector2Int, Vector2Int> GetMove()
    {
        return Move;
    }

    public override void Cleanup() {
        if (Server.has_instance) { Server.getInstance().Stop("Host force stop"); }
        if (Client.has_instance) { Client.getInstance().Stop(); }
    }
}