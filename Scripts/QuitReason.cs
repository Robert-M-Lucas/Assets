using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitReason : MonoBehaviour
{
    public string Reason = "";
    public bool Claimed = false;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
