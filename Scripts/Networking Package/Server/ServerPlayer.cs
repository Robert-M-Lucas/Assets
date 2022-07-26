using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class ServerPlayer : ServerPlayerExtraData
{
    public Socket Handler;

    public int ID;

    public string Name;

    public byte[] Buffer = new byte[1024];
    public byte[] LongBuffer = new byte[1024];
    public int CurrentPacketLength = -1;
    public int LongBufferSize = 0;
    public StringBuilder SB = new StringBuilder();

    public string GetUniqueString()
    {
        return "[" + ID + "] " + "'" + Name + "'";
    }

    public void Reset()
    {
        Buffer = new byte[1024];
        SB = new StringBuilder();
    }

    public ServerPlayer(Socket handler, int playerID)
    {
        Handler = handler;
        ID = playerID;
    }
}
