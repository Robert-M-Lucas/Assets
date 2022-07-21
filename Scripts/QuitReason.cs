using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitReason : MonoBehaviour
{
    public string reason = "";
    public bool claimed = false;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
