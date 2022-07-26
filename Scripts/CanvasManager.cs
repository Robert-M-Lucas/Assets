using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    public TMP_Text ChessClock;
    float _time = 0;

    bool escape_menu_active = false;

    //public CanvasGroup Chessboard2D;

    void Start()
    {
        
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
        chessManager.InitialiseBoard();
        chessManagerInterface.ResetManager();
        CheckText.transform.parent.gameObject.SetActive(false);
        CheckmateText.transform.parent.gameObject.SetActive(false);
        SetTurnText(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) { ToggleEscapeMenu(); }
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

        string time_text = "";
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
