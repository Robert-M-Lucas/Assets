using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public ChessManager chessManager;

    void Start()
    {
        chessManager.InitialiseBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
