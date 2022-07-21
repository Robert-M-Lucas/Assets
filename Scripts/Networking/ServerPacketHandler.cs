using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerPacketHandler : PacketHandlerParent
{
    public override Dictionary<int, Action<Packet>> UIDtoAction { get; } =
        new Dictionary<int, Action<Packet>>
        {
            { 201, (Packet p) => OnMoveRecieve(p) }
        };

    public ServerPacketHandler()
    {

    }

    public static void OnMoveRecieve(Packet packet)
    {
        ClientSendMovePacket movePacket = new ClientSendMovePacket(packet);
        // Debug.Log($"Forwarding move information: {movePacket.fromX} {movePacket.fromY} {movePacket.toX} {movePacket.toY}");
        Server.getInstance().SendMessage(Mathf.Abs(packet.From - 1), ServerSendMovePacket.Build(0, movePacket.fromX, movePacket.fromY, movePacket.toX, movePacket.toY));
    }
}
