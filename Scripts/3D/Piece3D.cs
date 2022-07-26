using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece3D : MonoBehaviour
{
    public string PieceType;
    public bool PieceColour;

    public bool Jump = false;
    public float JumpHeight = 3f;

    public GameObject Text;
    public GameObject Text2;

    [HideInInspector]
    public Vector3 MovingFrom;
    [HideInInspector]
    public Vector3 TargetPosition;
    [HideInInspector]
    public float MoveStartTime;
    [HideInInspector]
    public bool ForceJump = false;

    const float MOVE_TIME = 0.3f;

    public void ShowText(bool show)
    {
        Text.gameObject.SetActive(show);
        Text2.gameObject.SetActive(show);
    }

    private void Start()
    {
        // ShowText(false);
    }

    void Update()
    {
        if (MovingFrom != TargetPosition)
        {
            float timescale = (Time.time - MoveStartTime) / MOVE_TIME;

            if (timescale >= 1)
            {
                transform.localPosition = TargetPosition;
                MovingFrom = TargetPosition;
                ForceJump = false;
            }

            Vector3 new_pos = Vector3.Lerp(MovingFrom, TargetPosition, MathP.CosSmooth(timescale));

            // Jumpling for pieces such as horse or castleing
            if (Jump || ForceJump)
            {
                new_pos += Vector3.up * (Mathf.Sin(timescale * Mathf.PI)) * JumpHeight;
            }

            transform.localPosition = new_pos;
        }
    }
}
