using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Maths extensions
public static class MathP
{
    // Smooths a linear value from 0 -> 1 using a cos function
    public static float CosSmooth(float linear)
    {
        return (-Mathf.Cos(linear * Mathf.PI) + 1) / 2;
    }

    public static int BoolToInt(bool _bool)
    {
        if (_bool) { return 1; }
        return 0;
    }

    public static float Min(float[] floats)
    {
        float min = float.MaxValue;
        foreach (float f in floats)
        {
            if (f < min)
            {
                min = f;
            }
        }
        return min;
    }

    public static float Max(float[] floats)
    {
        float max = float.MinValue;
        foreach (float f in floats)
        {
            if (f > max)
            {
                max = f;
            }
        }
        return max;
    }
}
