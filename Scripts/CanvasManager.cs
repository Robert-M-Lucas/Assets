using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChessPieces;
using TMPro;
using System;

public class CanvasManager : MonoBehaviour
{
    [Header("References")]
    public ChessManager chessManager;
    public InputManager inputManager;
    public CameraControlScript cameraControl;
    public ChessManagerInterface chessManagerInterface;

    public GameObject EscapeMenu;

    public RectTransform cursor;

    public TMP_Text CheckmateText;
    public TMP_Text CheckText;
    public TMP_Text TurnText;

    public TMP_Text TurnProgressText;

    public TMP_Text EvaluationText;
    public TMP_Text WhiteLostText;
    public TMP_Text BlackLostText;

    public TMP_Text ChessClock;
    float _time = 0;

    bool escape_menu_active = false;

    //public CanvasGroup Chessboard2D;

    void Start()
    {
        
    }

    public void SetEvaluationText(string text)
    {
        EvaluationText.text = text;
    }

    public void UpdateEvaluation(ChessState state)
    {
        List<char>[] LostPieces = new List<char>[] { new List<char> { 'e', 'd', 'd', 'c', 'c', 'b', 'b', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a' },
                                                     new List<char> { 'e', 'd', 'd', 'c', 'c', 'b', 'b', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a' }, };
      
        int piece_score = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (state.Board[x, y] is null || state.Board[x, y].GetType() == typeof(King)) { continue; }

                int value = (int) state.Board[x, y].GetValue();
                int side = MathP.BoolToInt(state.Board[x, y].Side);

                if (!state.Board[x, y].Side) { value *= -1; }

                char to_remove;
                Type piece_type = state.Board[x, y].GetType();
                if (piece_type == typeof(Queen)) to_remove = 'e';
                else if (piece_type == typeof(Rook)) to_remove = 'd';
                else if (piece_type == typeof(Bishop)) to_remove = 'c';
                else if (piece_type == typeof(Knight)) to_remove = 'b';
                else to_remove = 'a';
                LostPieces[side].Remove(to_remove);

                piece_score += value;
            }
        }

        if (piece_score == 0)
        {
            EvaluationText.text = "Equal";
        }
        else if (piece_score > 0)
        {
            EvaluationText.text = $"White +{piece_score}";
        }
        else
        {
            EvaluationText.text = $"Black +{-piece_score}";
        }

        string white_lost = "";
        foreach (char piece in LostPieces[1]) { white_lost += piece; }
        WhiteLostText.text = white_lost;

        string black_lost = "";
        foreach (char piece in LostPieces[0]) { black_lost += piece; }
        BlackLostText.text = black_lost.ToUpper();
    }

    public void SetCheck(bool check)
    {
        if (CheckmateText.transform.parent.gameObject.activeInHierarchy)
        {
            CheckText.transform.parent.gameObject.SetActive(false);
            return;
        }
        CheckText.transform.parent.gameObject.SetActive(check);
    }

    public void SetTurnText(bool turn)
    {
        if (turn)
        {
            TurnText.text = "White's turn";
        }
        else
        {
            TurnText.text = "Black's turn";
        }
    }

    public void SetProgress(float progress)
    {
        if (progress <= 0) { TurnProgressText.gameObject.SetActive(false); return; }
        TurnProgressText.gameObject.SetActive(true);
        TurnProgressText.text = Mathf.Round(progress * 100) + "%";
    }

    public void SetCursorPosition(Vector2 cursor_pos)
    {
        cursor.position = cursor_pos;
    }

    public void ShowCheckmate()
    {
        CheckmateText.transform.parent.gameObject.SetActive(true);
        SetCheck(false);
    }

    public void ToggleEscapeMenu()
    {
        escape_menu_active = !escape_menu_active;
        EscapeMenu.SetActive(escape_menu_active);

        if (escape_menu_active)
        {
            inputManager.UnlockCursor();
        }
        else
        {
            inputManager.LockCursor();
        }
    }

    public void ExitToMenu()
    {
        ChessSettingsScript chessSettingsScript = FindObjectOfType<ChessSettingsScript>();
        chessSettingsScript.QuitToMainMenu();
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Restart()
    {
        if (!FindObjectOfType<ChessSettingsScript>().Restartable) return;
        chessManager.InitialiseBoard();
        chessManagerInterface.ResetManager();
        CheckText.transform.parent.gameObject.SetActive(false);
        CheckmateText.transform.parent.gameObject.SetActive(false);
        _time = 0;
        SetTurnText(true);
    }

    void Update()
    {
        if (I.GetKeyDown(K.RestartKey))
        {
            Restart();
        }

        if (I.GetKeyDown(K.EscapeMenuKey)) { ToggleEscapeMenu(); }
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;
        float temp_time = _time;

        int hours = (int) temp_time / 3600;
        temp_time %= 3600;

        int minutes = (int) temp_time / 60;
        temp_time %= 60;

        int seconds = (int) temp_time;

        string time_text = "<mspace=0.55em>";
        if (hours > 0)
        {
            if (hours < 10)
            {
                time_text += "0";
            }
            time_text += $"{hours}:";
        }
        
        if (minutes < 10)
        {
            time_text += "0";
        }
        time_text += $"{minutes}:";

        if (seconds < 10)
        {
            time_text += "0";
        }
        time_text += $"{seconds}";

        ChessClock.text = time_text;
    }
}
