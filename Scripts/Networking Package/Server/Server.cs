using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Server : ServerClientParent
{
    public string ServerPassword = "";

    public static bool is_running { private set; get; } = false;

    // public bool stopping = false;

    private Socket _handler;
    private Socket _listener;

    public string ServerInfo = "";
    public Action ServerInfoUpdateAction = () => { };

    # region Threads
    private Thread AcceptClientThread;
    public string AcceptClientThreadInfo = "";
    public Action AcceptClientUpdateAction = () => { };
    public Thread RecieveThread;
    public string RecieveThreadInfo = "";
    public Action RecieveUpdateAction = () => { };
    public Thread SendThread;
    public string SendThreadInfo = "";
    public Action SendUpdateAction = () => { };
    # endregion

    public Action OnPlayerJoinAction = () => { };
    public Action OnPlayerLeaveAction = () => { };

    public Dictionary<int, ServerPlayer> Players = new Dictionary<int, ServerPlayer>();

    public int UsersConnected
    {
        get { return Players.Count; }
    }

    ConcurrentQueue<Tuple<int, byte[]>> ContentQueue = new ConcurrentQueue<Tuple<int, byte[]>>();
    ConcurrentQueue<Tuple<int, byte[]>> SendQueue = new ConcurrentQueue<Tuple<int, byte[]>>();

    ConcurrentDictionary<int, Tuple<int, byte[]>> RequireResponse =
        new ConcurrentDictionary<int, Tuple<int, byte[]>>();
    ConcurrentQueue<int> RequiredResponseQueue = new ConcurrentQueue<int>();
    int RID = 1;
    CircularArray<int> RecievedRIDs = new CircularArray<int>(50);

    // Dictionary<string, Func<string, Server, int, bool>> PacketActions = new Dictionary<string, Func<string, Server, int, bool>>();

    public ServerClientHierachy hierachy;

    public bool AcceptingClients = false;

    int playerIDCounter = 0;

    // Singleton setup
    private Server()
    {
        hierachy = new ServerClientHierachy(this);
        DefaultHierachy.Add(new DefaultServerPacketHandler());
    }

    private static Server instance = null;
    public static bool has_instance
    {
        get { return !(instance is null); }
    }

    public static Server getInstance(bool instantiate = false)
    {
        if (instance is null && instantiate)
        {
            instance = new Server();
        }
        return instance;
    }

    public void Start(string _password = null)
    {
        if (_password != null)
        {
            ServerPassword = _password;
        }
        ServerLogger.ServerLog("Starting server");
        AcceptClientThread = new Thread(AcceptClients);
        AcceptClientThread.Start();
        RecieveThread = new Thread(RecieveLoop);
        RecieveThread.Start();
        SendThread = new Thread(SendLoop);
        SendThread.Start();
        is_running = true;
    }

    public ServerPlayer GetPlayer(int playerID)
    {
        if (Players.ContainsKey(playerID))
        {
            return Players[playerID];
        }
        return null;
    }

    ServerPlayer AddPlayer(Socket handler)
    {
        Players.Add(playerIDCounter, new ServerPlayer(handler, playerIDCounter));
        playerIDCounter++;
        new Thread(() => OnPlayerJoinAction()).Start();

        return Players[playerIDCounter - 1];
    }

    public void RemovePlayer(int playerID, string reason)
    {
        // TODO: Figure out a way to send a kick packet to a player despite them being disconnected immediately
        try
        {
            Players[playerID].Handler.Send(ServerKickPacket.Build(0, reason));
            Players[playerID].Handler.Shutdown(SocketShutdown.Both);
            Players[playerID].Handler.Close();
            Players[playerID].Handler.Dispose();
        }
        catch (Exception e)
        {

        }
        Players.Remove(playerID);

        foreach (int otherPlayerID in Players.Keys)
        {
            SendMessage(playerID, ServerInformOfClientDisconnectPacket.Build(0, playerID), false);
        }

        new Thread(() => OnPlayerLeaveAction()).Start();

    }

    public void UpdateAllPlayersAboutPlayer(ServerPlayer playerAbout)
    {
        foreach (int playerID in Players.Keys)
        {
            if (playerID == playerAbout.ID)
            {
                continue;
            }
            SendMessage(
                playerID,
                ServerOtherClientInfoPacket.Build(0, playerAbout.ID, playerAbout.Name),
                false
            );
        }
    }

    public void UpdatePlayerAboutAllPlayers(ServerPlayer playerUpdated)
    {
        foreach (int playerID in Players.Keys)
        {
            if (playerID == playerUpdated.ID)
            {
                continue;
            }
            ServerPlayer player = Players[playerID];
            SendMessage(
                playerUpdated.ID,
                ServerOtherClientInfoPacket.Build(0, player.ID, player.Name),
                false
            );
        }
    }

    // Accept client
    void AcceptClients()
    {
        ServerLogger.ServerLog("Server Client Accept Thread Start");

        IPAddress ipAddress = IPAddress.Any;

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, NetworkSettings.PORT);

        try
        {
            _listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            _listener.Bind(localEndPoint);

            _listener.Listen(100);

            while (true)
            {
                ServerLogger.AC("SERVER: Waiting for a connection...");
                Socket handler = _listener.Accept();
                _handler = handler;
                ServerLogger.AC("SERVER: Client connecting");

                // Incoming data from the client.
                byte[] rec_bytes = new byte[1024];
                int total_rec = 0;

                while (total_rec < 4)
                {
                    byte[] partial_bytes = new byte[1024];
                    int bytesRec = handler.Receive(rec_bytes);

                    total_rec += bytesRec;

                    // string _out2 = "";
                    // for (int i = 0; i < rec_bytes.Length; i++){
                    //     _out2 += rec_bytes[i].ToString() + ":";
                    // }
                    // Debug.Log(_out2);

                    Tuple<byte[], int> cleared = ArrayExtentions.ClearEmpty(rec_bytes);
                    rec_bytes = cleared.Item1;
                    total_rec -= cleared.Item2;

                    ArrayExtentions.Merge(
                        rec_bytes,
                        ArrayExtentions.Slice(partial_bytes, 0, bytesRec),
                        total_rec
                    );
                }

                int packet_len = PacketBuilder.GetPacketLength(
                    ArrayExtentions.Slice(rec_bytes, 0, 4)
                );

                while (total_rec < packet_len)
                {
                    byte[] partial_bytes = new byte[1024];
                    int bytesRec = handler.Receive(partial_bytes);

                    total_rec += bytesRec;
                    ArrayExtentions.Merge(
                        rec_bytes,
                        ArrayExtentions.Slice(partial_bytes, 0, bytesRec),
                        total_rec
                    );
                }

                ClientConnectRequestPacket initPacket = new ClientConnectRequestPacket(
                    PacketBuilder.Decode(ArrayExtentions.Slice(rec_bytes, 0, packet_len))
                );

                if (!AcceptingClients)
                {
                    handler.Send(
                        ServerKickPacket.Build(0, "Server is not accepting clients at this time")
                    );
                    ServerLogger.AC("SERVER: Client kicked - not accepting clients");
                    continue;
                }

                if (ServerPassword != "" && initPacket.Password != ServerPassword)
                {
                    handler.Send(
                        ServerKickPacket.Build(0, "Wrong Password: '" + initPacket.Password + "'")
                    );
                    ServerLogger.AC("SERVER: Client kicked - wrong password");
                    continue;
                }

                // Version mismatch
                if (initPacket.Version != NetworkSettings.VERSION)
                {
                    handler.Send(
                        ServerKickPacket.Build(
                            0,
                            "Wrong Version:\nServer: "
                                + NetworkSettings.VERSION.ToString()
                                + "| Client (You): "
                                + initPacket.Version
                        )
                    );
                    ServerLogger.AC(
                        "SERVER: Client kicked - Wrong Version - Server: "
                            + NetworkSettings.VERSION.ToString()
                            + " Client: "
                            + initPacket.Version
                    );
                    continue;
                }

                // TODO: Add player join logic
                ServerPlayer player = AddPlayer(handler);
                player.Name = initPacket.Name;

                foreach (Action<ServerPlayer> action in hierachy.OnPlayerJoinActions)
                {
                    action(player);
                }

                SendMessage(player.ID, ServerConnectAcceptPacket.Build(0, player.ID), false);

                UpdateAllPlayersAboutPlayer(player);
                UpdatePlayerAboutAllPlayers(player);

                ServerLogger.AC(
                    "Player " + player.GetUniqueString() + " connected. Beginning recieve"
                );

                handler.BeginReceive(
                    player.Buffer,
                    0,
                    1024,
                    0,
                    new AsyncCallback(ReadCallback),
                    player
                );
            }
        }
        catch (ThreadAbortException) {  }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            ServerLogger.AC("[ERROR] " + e.ToString());
        }
    }

    public void SendMessage(int ID, byte[] message, bool require_response = false)
    {
        SendQueue.Enqueue(new Tuple<int, byte[]>(ID, message));

        if (require_response)
        {
            RequireResponse[RID] = new Tuple<int, byte[]>(ID, message);
            RequiredResponseQueue.Enqueue(RID);
            RID++;
        }
    }

    void SendLoop()
    {
        try
        {
            while (!Stopping)
            {
                if (!SendQueue.IsEmpty)
                {
                    Tuple<int, byte[]> to_send;
                    if (SendQueue.TryDequeue(out to_send))
                    {
                        ServerLogger.S(
                            "To " + Players[to_send.Item1].GetUniqueString() + "; Sent packet"
                        );
                        try
                        {
                            Players[to_send.Item1].Handler.Send(to_send.Item2);
                        }
                        catch (SocketException se)
                        {
                            ServerLogger.S(
                                "Client: "
                                    + Players[to_send.Item1].GetUniqueString()
                                    + " disconnected due to socket exception: "
                                    + se
                            );
                            Players.Remove(to_send.Item1);
                        }
                    }
                }
                else if (!RequiredResponseQueue.IsEmpty)
                {
                    int rid;
                    if (RequiredResponseQueue.TryDequeue(out rid))
                    {
                        if (RequireResponse.ContainsKey(rid))
                        {
                            Tuple<int, byte[]> to_send = RequireResponse[rid];
                            ServerLogger.S(
                                "To "
                                    + Players[to_send.Item1].GetUniqueString()
                                    + "; Sent RID packet"
                            );
                            Players[to_send.Item1].Handler.Send(to_send.Item2);
                            RequiredResponseQueue.Enqueue(rid);
                        }
                    }
                }

                Thread.Sleep(2);
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception e)
        {
            Debug.LogError(e);
            ServerLogger.S("[ERROR] " + e.ToString());
        }
    }

    private void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        ServerPlayer CurrentPlayer = (ServerPlayer)ar.AsyncState;
        Socket handler = CurrentPlayer.Handler;

        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            ArrayExtentions.Merge(
                CurrentPlayer.LongBuffer,
                CurrentPlayer.Buffer,
                CurrentPlayer.LongBufferSize
            );
            CurrentPlayer.LongBufferSize += bytesRead;

            ReprocessBuffer:

            if (
                CurrentPlayer.CurrentPacketLength == -1
                && CurrentPlayer.LongBufferSize >= PacketBuilder.PacketLenLen
            )
            {
                CurrentPlayer.CurrentPacketLength = PacketBuilder.GetPacketLength(
                    CurrentPlayer.LongBuffer
                );
            }

            if (
                CurrentPlayer.CurrentPacketLength != -1
                && CurrentPlayer.LongBufferSize >= CurrentPlayer.CurrentPacketLength
            )
            {
                ServerLogger.R("Recieved Packet from " + CurrentPlayer.GetUniqueString());
                ContentQueue.Enqueue(
                    new Tuple<int, byte[]>(
                        CurrentPlayer.ID,
                        ArrayExtentions.Slice(
                            CurrentPlayer.LongBuffer,
                            0,
                            CurrentPlayer.CurrentPacketLength
                        )
                    )
                );
                byte[] new_buffer = new byte[1024];
                ArrayExtentions.Merge(
                    new_buffer,
                    ArrayExtentions.Slice(
                        CurrentPlayer.LongBuffer,
                        CurrentPlayer.CurrentPacketLength,
                        1024
                    ),
                    0
                );
                CurrentPlayer.LongBuffer = new_buffer;
                CurrentPlayer.LongBufferSize -= CurrentPlayer.CurrentPacketLength;
                CurrentPlayer.CurrentPacketLength = -1;
                if (CurrentPlayer.LongBufferSize > 0)
                {
                    goto ReprocessBuffer;
                }
            }

            // ContentQueue.Enqueue(new Tuple<int, byte[]>(CurrentPlayer.ID, subcontent));
            // CurrentPlayer.Reset(); // Reset buffers
            //
            handler.BeginReceive(
                CurrentPlayer.Buffer,
                0,
                1024,
                0,
                new AsyncCallback(ReadCallback),
                CurrentPlayer
            ); // Listen again
            // }
            // else
            // {
            //     // Not all data received. Get more.
            //     handler.BeginReceive(CurrentPlayer.buffer, 0, 1024, 0,
            //     new AsyncCallback(ReadCallback), CurrentPlayer);
            // }
        }
        else
        {
            handler.BeginReceive(
                CurrentPlayer.Buffer,
                0,
                1024,
                0,
                new AsyncCallback(ReadCallback),
                CurrentPlayer
            );
        }
    }

    void RecieveLoop()
    {
        try
        {
            while (!Stopping)
            {
                if (ContentQueue.IsEmpty)
                {   
                    Thread.Sleep(2);
                    continue;
                } // Nothing recieved

                Tuple<int, byte[]> content;
                if (!ContentQueue.TryDequeue(out content))
                {
                    continue;
                }
                if (!Players.ContainsKey(content.Item1))
                {
                    ServerLogger.R(
                        "Packet from player with ID "
                            + content.Item1
                            + " not handled as they have been disconnected"
                    );
                    continue;
                }

                ServerLogger.R("Handling Packet");
                try
                {
                    bool handled = hierachy.HandlePacket(content.Item2, content.Item1);

                    if (!handled)
                    {
                        ServerLogger.R(
                            "[ERROR] Failed to handle packed with UID "
                                + PacketBuilder.Decode(content.Item2).UID
                                + ". Probable hierachy error"
                        );
                    }
                }
                catch (PacketDecodeError e)
                {
                    ServerLogger.R(
                        "[ERROR] "
                            + "Error handling packet from "
                            + Players[content.Item1].GetUniqueString()
                            + "; Error: "
                            + e.ToString()
                    );
                    // TODO: Disconnect client
                    RemovePlayer(content.Item1, "Fatal packet handling error");
                }
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception e)
        {
            Debug.LogError(e);
            ServerLogger.R("[ERROR] " + e.ToString());
        }
    }

    public void Stop(string reason)
    {
        ServerLogger.ServerLog("Server Shutting Down");
        Stopping = true;
        
        Thread.Sleep(5);
        try { AcceptClientThread.Abort(); } catch (Exception e) { }//Debug.Log(e); }
        try { RecieveThread.Abort(); } catch (Exception e) { }//Debug.Log(e); }
        try { SendThread.Abort(); } catch (Exception e) { }//Debug.Log(e); }
        foreach (ServerPlayer player in Players.Values)
        {
            if (reason is null) try { player.Handler.Send(ServerKickPacket.Build(0, "Server shutting down. No reason given")); } catch (Exception e) { }//Debug.Log(e); }
            else try { player.Handler.Send(ServerKickPacket.Build(0, $"Server shutting down. Reason: {reason}")); } catch (Exception e) { }//Debug.Log(e); }

            try { player.Handler.Shutdown(SocketShutdown.Both); player.Handler.Close(); player.Handler.Dispose(); } catch (Exception e) { Debug.Log(e); }
        }

        try { _handler?.Shutdown(SocketShutdown.Both); 
            _handler?.Close();
            _handler?.Dispose(); 
        } catch (Exception e) { Debug.Log(e); }
        Debug.Log(_listener);

        try { _listener?.Shutdown(SocketShutdown.Both); } catch (Exception e) { }// { Debug.Log(e); }
        try { _listener?.Disconnect(false); } catch (Exception e) { } // { Debug.Log(e); }
        try { _listener?.Close(); } catch (Exception e) { Debug.Log(e); }
        try { _listener?.Dispose(); } catch (Exception e) { Debug.Log(e); }
        
        Debug.Log(_listener.Connected);
        instance = null;
        is_running = false;
        ServerLogger.ServerLog("Server Shut Down Complete");
    }
}
