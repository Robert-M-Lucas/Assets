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
            if (quitReason.Claimed && quitReason.Reason != "")
            {
                mainMenuCanvasManager.ShowJoinScreen();
                mainMenuCanvasManager.JoinInfoString = quitReason.Reason;
                Destroy(quitReason.gameObject);
            }
            else
            {
                quitReason.Claimed = true;
            }
        }
    }

    void Play()
    {
        locked = true;
        mainMenuCanvasManager.StartSceneChange();
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
        JoinSide = -1;
        Play();
    }

    public void JoinFailed(string reason)
    {
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
        Server.getInstance().OnPlayerLeaveAction = PlayerLeftHost;
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
        Server.getInstance().Stop("Host force stop");
        Client.getInstance()?.Stop();
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

    void PlayerLeftHost()
    {
        QuitToMenuNextFrame = true;
        QuitToMenuReason = "Opponent left";
    }

    public void QuitToMainMenu(string reason = null)
    {
        PlayerHasJoined = false;
        JoinSide = -1;
        locked = false;
        QuitReason quitReason = FindObjectOfType<QuitReason>();
        if (reason is null)
        {
            DestroyImmediate(quitReason.gameObject);
        }
        else
        {
            quitReason.Reason = reason;
        }
        
        if (Server.has_instance) { Server.getInstance().Stop("Host force stop"); }
        if (Client.has_instance) { Client.getInstance().Stop(); }
        OnApplicationQuit();
        FadeOutScript fadeOut = FindObjectOfType<FadeOutScript>();
        if (fadeOut is not null)
        {
            fadeOut.Fade = true;
            // StartCoroutine(DelayedMenuChange());
        }
        else
        {
            SceneManager.LoadScene(0);
        }

        DestroyImmediate(gameObject);
    }

    /*
    IEnumerator DelayedMenuChange()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Load scene?");
        SceneManager.LoadScene(0);
    }
    */

    public void Update()
    {
        if (locked)
        {
            Debug.Log(JoinSide);
            if (JoinSide != -1) { JoinSuccessful(); }
            if (PlayerHasJoined) { HostSuccessful(); } 
        }
        if (QuitToMenuNextFrame)
        {
            QuitToMainMenu(QuitToMenuReason);
        }
    }
}
