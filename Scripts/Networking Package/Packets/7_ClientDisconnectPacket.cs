using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ClientDisconnectPacket {
    public const int UID = 7;
    public int RID;
    public ClientDisconnectPacket(Packet packet){
        RID = packet.RID;
    }

    public static byte[] Build(int _RID) {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents, _RID);
    }
}