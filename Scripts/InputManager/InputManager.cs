using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class I
{
    public static bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }
    public static bool GetKey(KeyCode[] keys)
    {
        foreach (var key in keys) { if (!Input.GetKey(key)) return false; }
        return true;
    }
    public static bool GetKey(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKey(key_combination)) return true;
        }
        return false;
    }
    public static bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
    public static bool GetKeyDown(KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (!Input.GetKey(keys[i])) return false;
        }
        
        return Input.GetKeyDown(keys[keys.Length - 1]);
    }
    public static bool GetKeyDown(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKeyDown(key_combination)) return true;
        }
        return false;
    }
    public static bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }
    public static bool GetKeyUp(KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (!Input.GetKey(keys[i])) return false;
        }

        return Input.GetKeyUp(keys[keys.Length - 1]);
    }
    public static bool GetKeyUp(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKeyUp(key_combination)) return true;
        }
        return false;
    }

    public static bool GetMouseButton(int button) => Input.GetMouseButton(button);

    public static bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);

    public static bool GetMouseButtonUp(int button) => Input.GetMouseButtonUp(button);
}
