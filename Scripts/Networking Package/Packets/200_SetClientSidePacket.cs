using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SetClientSidePacket {
    public const int UID = 200;
    public int RID;
    public int side;
    public SetClientSidePacket(Packet packet){
        RID = packet.RID;
        side = BitConverter.ToInt32(packet.Contents[0]);
    }

    public static byte[] Build(int _RID, int _side) {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_side));
            return PacketBuilder.Build(UID, contents, _RID);
    }
}