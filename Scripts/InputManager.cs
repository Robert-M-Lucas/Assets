using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 look_sensitivity = Vector2.one;
    public Vector2 mouse_sensitivity = Vector2.one;

    [Header("References")]
    public LayerMask Square3DMask;
    public CameraControlScript cameraControl;
    public CanvasManager canvasManager;
    public AppearanceManager3D appearanceManager3D;
    public Chessboard3DPieceManager chessboard3DPieceManager;
    public ChessManagerInterface chessManagerInterface;
    public SoundManager soundManager;

    // Internal

    [HideInInspector]
    public bool PerspectiveMode = true;
    [HideInInspector]
    public bool EdgeLit = false;
    Vector2 mousePosition = Vector2.zero;

    void Start()
    {
        LockCursor();
    }

    bool cursor_locked;

    void HandleHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, Square3DMask))
        {
            appearanceManager3D.hoveringOver = hit.collider.gameObject.GetComponent<Chessboard3DSquare>().position;
            if (Input.GetMouseButtonDown(0))
            {
                chessManagerInterface.ChangeSelected(appearanceManager3D.hoveringOver);
            }
        }
        else
        {
            HandleNoHit();
        }
    }

    // Deselect any selected tiles
    void HandleNoHit()
    {
        appearanceManager3D.hoveringOver = new Vector2Int(-1, -1);
    }

    
    void Update()
    {
        // Switch from 3D to 2D
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PerspectiveMode = !PerspectiveMode;
            chessboard3DPieceManager.UpdateTheme();

            chessboard3DPieceManager.ShowText(!PerspectiveMode);

            if (!PerspectiveMode)
            {
                HandleNoHit();
                
            }
        }

        // Toggle edge lighting
        if (Input.GetKeyDown(KeyCode.E))
        {
            EdgeLit = !EdgeLit;
            chessboard3DPieceManager.UpdateTheme();
        }

        // If cursor is unlocked it means UI is being used
        if (!cursor_locked) { return; }

        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(1) && PerspectiveMode)
        {
            cameraControl.RotateCamera(new Vector2(mouse_x * look_sensitivity.x, mouse_y * look_sensitivity.y));
            HandleNoHit();
        }
        else
        {
            // Move virtual mouse
            mousePosition += new Vector2(mouse_x * mouse_sensitivity.x, mouse_y * mouse_sensitivity.y);

            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            if (!cursor_locked)
            {
                mousePosition = Input.mousePosition;
            }

            canvasManager.SetCursorPosition(mousePosition);

            // Only allow 3D tile hits if in 3D mode
            //if (PerspectiveMode)
            //{
            
            HandleHit();
            //}
        }
    }

    public void LockCursor()
    {
        mousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursor_locked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursor_locked = false;
    }

}
