using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ClientPacketHandler : PacketHandlerParent
{
    public override Dictionary<int, Action<Packet>> UIDtoAction { get; } =
        new Dictionary<int, Action<Packet>>
        {
            { 200, (Packet p) => OnSideRecieve(p) },
            { 202, (Packet p) => OnMoveRecieve(p) }
        };

    public static OnlineClientTurnHandler clientTurnHandler;

    public ClientPacketHandler()
    {

    }

    public static void OnSideRecieve(Packet packet)
    {
        // Debug.Log("Recieved side");
        SetClientSidePacket clientSidePacket = new SetClientSidePacket(packet);
        ChessSettingsScript.JoinSide = clientSidePacket.side;
    }

    public static void OnMoveRecieve(Packet packet)
    {
        ServerSendMovePacket clientSidePacket = new ServerSendMovePacket(packet);
        // Debug.Log($"Recieved move information: {clientSidePacket.fromX} {clientSidePacket.fromY} {clientSidePacket.toX} {clientSidePacket.toY}");
        clientTurnHandler.move = new Tuple<Vector2Int, Vector2Int>(new Vector2Int(clientSidePacket.fromX, clientSidePacket.fromY), new Vector2Int(clientSidePacket.toX, clientSidePacket.toY));
        clientTurnHandler.waiting_for_move = false;
    }
}
