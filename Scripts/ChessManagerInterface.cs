using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManagerInterface : MonoBehaviour
{
    [Header("References")]
    public ChessManager chessManager;
    public CanvasManager canvasManager;
    public InputManager inputManager;
    public SoundManager soundManager;

    [HideInInspector]
    public List<Tuple<Vector2Int, bool>> possibleMoves = new List<Tuple<Vector2Int, bool>>();

    [HideInInspector]
    public Vector2Int Selected = -Vector2Int.one;

    public static readonly Vector2Int UNSELECTED = new Vector2Int(-1, -1);

    
    ChessSettingsScript chessSettings;

    public void Start()
    {
        chessSettings = FindObjectOfType<ChessSettingsScript>();

        if (chessSettings.turnHandlers[1] is not null)
        {
            chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)].StartFindingMove(chessManager.State);
        }

        if (chessSettings.turnHandlers[1] is not null && chessSettings.turnHandlers[0] is null)
        {
            inputManager.cameraControl.RotateCamera(new Vector2(180, 0));
        }
    }

    public bool MoveExists(Vector2Int target_move, out bool check)
    {
        check = false;

        foreach (Tuple<Vector2Int, bool> move in possibleMoves)
        {
            if (move.Item1 == target_move)
            {
                check = move.Item2;
                return true;
            }
        }
        return false;
    }

    public void ResetManager()
    {
        possibleMoves.Clear();
        Selected = -Vector2Int.one;

        if (chessSettings.turnHandlers[1] is not null)
        {
            chessSettings.turnHandlers[1].StartFindingMove(chessManager.State);
        }
    }

    // Change selected tile
    public void ChangeSelected(Vector2Int new_selected)
    {
        if (chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)] is not null)
        {
            Selected = UNSELECTED;
            return;
        }

        // Tile reselected
        if (new_selected == Selected || (Selected != UNSELECTED && !MoveExists(new_selected, out _)))
        {
            soundManager.PlayOffClick();
            Selected = UNSELECTED;
            possibleMoves.Clear();
            return;
        }

        // Move piece
        if (new_selected.x != -1 && Selected.x != -1)
        {
            bool check;
            if (MoveExists(new_selected, out check)) {
                // MOVE
                Move(Selected, new_selected, check);
                possibleMoves.Clear();
                Selected = UNSELECTED;
                return;
            }      
        }

        possibleMoves = chessManager.GetMovesFromAllMoves(new_selected);
        if (possibleMoves.Count > 0)
        {
            soundManager.PlayOnClick();
            Selected = new_selected;
        }
    }

    public void Move(Vector2Int pos1, Vector2Int pos2, bool check = false)
    {
        chessManager.TryMovePiece(pos1, pos2);
        canvasManager.SetCheck(check);
        canvasManager.SetTurnText(chessManager.State.turn);

        if (chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)] is not null)
        {
            chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)].StartFindingMove(chessManager.State);
        }

        soundManager.PlayPieceMoveSound(new Vector3(pos2.x - 3.5f, 0, 7 - (pos2.y + 3.5f)));
    }

    public void Update()
    {
        if (chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)] is not null && chessManager.State.Checkmate == -1)
        {
            TurnHandlerInterface turnHandler = chessSettings.turnHandlers[MathP.BoolToInt(chessManager.State.turn)];
            if (turnHandler.MoveAvailable())
            {
                Tuple<Vector2Int, Vector2Int> move = turnHandler.GetMove();
                Move(move.Item1, move.Item2);
            }
            else
            {
                float progress = turnHandler.GetMoveFindProgress();
                canvasManager.SetProgress(progress);
            }
        }
        else
        {
            canvasManager.SetProgress(-1);
        }
    }
}
