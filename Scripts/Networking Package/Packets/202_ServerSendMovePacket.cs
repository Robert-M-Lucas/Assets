using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerSendMovePacket {
    public const int UID = 202;
    public int RID;
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
    public ServerSendMovePacket(Packet packet){
        RID = packet.RID;
        fromX = BitConverter.ToInt32(packet.contents[0]);
        fromY = BitConverter.ToInt32(packet.contents[1]);
        toX = BitConverter.ToInt32(packet.contents[2]);
        toY = BitConverter.ToInt32(packet.contents[3]);
    }

    public static byte[] Build(int _RID, int _fromX, int _fromY, int _toX, int _toY) {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_fromX));
            contents.Add(BitConverter.GetBytes(_fromY));
            contents.Add(BitConverter.GetBytes(_toX));
            contents.Add(BitConverter.GetBytes(_toY));
            return PacketBuilder.Build(UID, contents, _RID);
    }
}