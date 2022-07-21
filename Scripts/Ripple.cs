using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles one ripple effect allowing multiple to be stacked
public class RippleData
{
    public Vector2Int position;
    public int frame_num;
    public float last_time;

    private float[,] current_frame;

    public RippleData(Vector2Int position)
    {
        this.position = position;
        frame_num = 0;
        last_time = Time.time;

        current_frame = Ripple.GetFrame(position, 0);
    }

    public float[,] GetFrame()
    {
        if (Time.time - Ripple.FRAME_TIME_INCREMENT > last_time)
        {
            last_time = Time.time;
            frame_num++;
            current_frame = Ripple.GetFrame(position, frame_num);
        }

        return current_frame;
    }
}

public static class Ripple
{
    public const int FRAME_COUNT = 20;
    public const float FRAME_TIME_INCREMENT = 0.1f;

    public static float[,] GetFrame(Vector2Int position, int frame_num)
    {
        float distance = (frame_num * FRAME_TIME_INCREMENT + 1) * 5;
        float magnitude = (Mathf.Cos((frame_num/(float)FRAME_COUNT) * Mathf.PI)+1)/2;

        float[,] frame = new float[8, 8];

        for (int x = 0; x < 8; x ++)
        {
            for (int y = 0; y < 8; y++)
            {
                float dist = Mathf.Sqrt(((x - position.x) * (x - position.x)) + ((y - position.y) * (y - position.y)));
                if (dist > distance) { continue; }
                float dist_multiplier = dist / distance;
                float ripple = Mathf.Sin(dist_multiplier * Mathf.PI * 2);

                frame[x, y] = ripple * dist_multiplier * magnitude * 0.5f;
            }
        }

        return frame;
    }
}
