using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a class
/// </summary>
public class QuitReason : MonoBehaviour
{
    public string reason = "";
    public bool claimed = false;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
