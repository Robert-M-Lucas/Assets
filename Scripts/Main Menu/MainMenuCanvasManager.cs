using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net;
using System;
using System.Threading;
using System.Net.Sockets;

public class MainMenuCanvasManager : MonoBehaviour
{
    public GameObject JoinScreen;

    public TMP_InputField JoinIPInput;
    public TMP_Text JoinInfo;
    public string JoinInfoString;

    public GameObject HostScreen;

    public TMP_Text LocalIP;
    public TMP_Text PublicIP;

    bool PublicIPShown = false;
    string PublicIPString = null;

    public CanvasGroup canvasGroup;
    float scene_change_progress = 0;
    bool scene_change = false;
    public float SceneChangeTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        try
        {
            LocalIP.text = $"Local IP: {GetLocalIPAddress()}";
        }
        catch (WebException)
        {
            LocalIP.text = "Local IP: NOT FOUND!";
        }

        new Thread(GetPublicIP).Start();
    }

    void GetPublicIP()
    {
        string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
        var externalIp = IPAddress.Parse(externalIpString);

        PublicIPString = externalIp.ToString();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new WebException("No network adapters with an IPv4 address in the system!");
    }

    public void ShowJoinScreen()
    {
        JoinScreen.SetActive(true);
    }
    public void HideJoinScreen()
    {
        JoinScreen.SetActive(false);
    }

    public void ShowHostScreen()
    {
        HostScreen.SetActive(true);
    }
    public void HideHostScreen()
    {
        HostScreen.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void StartSceneChange()
    {
        scene_change = true;
    }

    // Update is called once per frame
    void Update()
    {
        JoinInfo.text = JoinInfoString;

        if (!PublicIPShown && PublicIPString is not null)
        {
            PublicIP.text = $"Public IP: {PublicIPString}";
            PublicIPShown = true;
        }

        if (scene_change)
        {
            scene_change_progress += Time.deltaTime / SceneChangeTime;
            if (scene_change_progress > 1)
            {
                SceneManager.LoadScene(1);
                return;
            }
            canvasGroup.alpha = 1 - scene_change_progress;
        }
    }
}
