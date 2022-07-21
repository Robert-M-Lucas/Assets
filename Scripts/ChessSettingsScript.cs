using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ChessSettingsScript : MonoBehaviour
{
    public TurnHandlerInterface[] turnHandlers = new TurnHandlerInterface[] { null, null };

    public MainMenuCanvasManager mainMenuCanvasManager;

    public static int JoinSide = -1;
    public static bool PlayerHasJoined = false;

    int HostSide = -1;

    bool locked = false;

    bool QuitToMenuNextFrame = false;
    string QuitToMenuReason = "";

    void Awake()
    {
        NetworkSettings.MainThreadStart();
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        QuitReason[] quitReasons = FindObjectsOfType<QuitReason>();
        foreach (QuitReason quitReason in quitReasons)
        {
            if (quitReason.claimed && quitReason.reason != "")
            {
                mainMenuCanvasManager.ShowJoinScreen();
                mainMenuCanvasManager.JoinInfoString = quitReason.reason;
                Destroy(quitReason.gameObject);
            }
            else
            {
                quitReason.claimed = true;
            }
        }
    }

    void Play()
    {
        SceneManager.LoadScene(1);
        locked = false;
    }

    public void LocalPlay()
    {
        if (locked) return;
        turnHandlers = new TurnHandlerInterface[] { null, null };
        Play();
    }

    public void Join()
    {
        Debug.Log(Client.has_instance);
        if (locked) return;
        string IP = mainMenuCanvasManager.JoinIPInput.text;
        Client.getInstance(true).hierachy.Hierachy.Add(new ClientPacketHandler());
        // Client.getInstance().ClientInfoUpdateAction = () => Debug.Log(Client.getInstance().ClientInfo);
        Client.getInstance().DisconnectAction = PlayerKicked;
        Client.getInstance().Connect(IP, "", "", WaitForSide, JoinFailed);
        mainMenuCanvasManager.JoinInfoString = "Connecting...";
        locked = true;
    }

    public void WaitForSide()
    {

    }

    public void JoinSuccessful()
    {
        if (JoinSide == 0)
        {
            turnHandlers = new TurnHandlerInterface[] { null, new OnlineClientTurnHandler() };
            ClientPacketHandler.clientTurnHandler = (OnlineClientTurnHandler)turnHandlers[1];
        }
        else
        {
            turnHandlers = new TurnHandlerInterface[] { new OnlineClientTurnHandler(), null };
            ClientPacketHandler.clientTurnHandler = (OnlineClientTurnHandler)turnHandlers[0];
        }
        Play();
    }

    public void JoinFailed(string reason)
    {
        Debug.Log(reason);
        locked = false;
        mainMenuCanvasManager.JoinInfoString = "Join failed due to:\n" + reason;
        Client.getInstance().Stop();
    }

    public void WhiteVsAi()
    {
        if (locked) return;
        turnHandlers = new TurnHandlerInterface[] { new AITurnHandler(), null };
        Play();
    }

    public void BlackVsAi()
    {
        if (locked) return;
        turnHandlers = new TurnHandlerInterface[] { null, new AITurnHandler() };
        Play();
    }

    public void HostAsWhite()
    {
        if (locked) return;
        HostSide = 1;
        Host();
    }

    public void HostAsBlack()
    {
        if (locked) return;
        HostSide = 0;
        Host();
    }

    public void Host()
    {
        locked = true;
        mainMenuCanvasManager.ShowHostScreen();
        Server.getInstance(true).AcceptingClients = true;
        Server.getInstance().OnPlayerJoinAction = () => { PlayerHasJoined = true; };
        Server.getInstance().hierachy.Hierachy.Add(new ServerPacketHandler());
        Server.getInstance().Start();
    }

    public static void PlayerJoined()
    {
        PlayerHasJoined = true;
        Server.getInstance().AcceptingClients = false;
    }

    public void HostSuccessful()
    {
        Server.getInstance().SendMessage(0, SetClientSidePacket.Build(0, Mathf.Abs(HostSide - 1)));
        Server.getInstance().AcceptingClients = true;
        // Server.getInstance().RecieveUpdateAction = () => Debug.Log(Server.getInstance().RecieveThreadInfo);

        Client.getInstance(true).ConnectThreaded("127.0.0.1");
        // Client.getInstance().SendUpdateAction = () => Debug.Log(Client.getInstance().SendThreadInfo);
        Client.getInstance().hierachy.Hierachy.Add(new ClientPacketHandler());

        Server.getInstance().AcceptingClients = false;
        if (HostSide == 0)
        {
            turnHandlers = new TurnHandlerInterface[] { null, new OnlineClientTurnHandler() };
            ClientPacketHandler.clientTurnHandler = (OnlineClientTurnHandler)turnHandlers[1];
        }
        else
        {
            turnHandlers = new TurnHandlerInterface[] { new OnlineClientTurnHandler(), null };
            ClientPacketHandler.clientTurnHandler = (OnlineClientTurnHandler)turnHandlers[0];
        }
        Play();
    }

    public void CancelHost()
    {
        if (!locked) { return; }
        Server.getInstance().Stop();
        mainMenuCanvasManager.HideHostScreen();
        PlayerHasJoined = false;
        HostSide = -1;
        locked = false;
    }

    void PlayerKicked(string reason)
    {
        QuitToMenuReason = reason;
        QuitToMenuNextFrame = true;
    }

    public void AiVsAi()
    {
        if (locked) return;
        turnHandlers = new TurnHandlerInterface[] { new AITurnHandler(), new AITurnHandler() };
        Play();
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Quit");
        if (turnHandlers[0] is not null) { turnHandlers[0].Cleanup(); }
        if (turnHandlers[1] is not null) { turnHandlers[1].Cleanup(); }
    }

    void QuitToMainMenu(string reason)
    {
        FindObjectOfType<QuitReason>().reason = reason;
        if (Server.has_instance) { Server.getInstance().Stop(); }
        if (Client.has_instance) { Client.getInstance().Stop(); }
        OnApplicationQuit();
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (locked)
        {
            if (JoinSide != -1) { JoinSuccessful(); }
            if (PlayerHasJoined) { HostSuccessful(); } 
        }
        if (QuitToMenuNextFrame)
        {
            QuitToMainMenu(QuitToMenuReason);
        }
    }
}
