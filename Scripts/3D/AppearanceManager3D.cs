using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ThemePack
{
    public string Name;

    public Material blackMaterial;
    public Material blackEdgeMaterial;
    public Material whiteMaterial;
    public Material whiteEdgeMaterial;
    public Material blackSquareMaterial;
    public Material whiteSquareMaterial;
    public Material blackHighlightMaterial;
    public Material whiteHighlightMaterial;
    public Material blackKillMaterial;
    public Material whiteKillMaterial;

    public ThemePack (string Name, Material blackMaterial, Material whiteMaterial, Material blackHighlightMaterial, Material whiteHighlightMaterial,
        Material blackKillMaterial, Material whiteKillMaterial)
    {
        this.Name = Name;
        this.blackMaterial = blackMaterial;
        this.whiteMaterial = whiteMaterial;
        this.blackHighlightMaterial = blackHighlightMaterial;
        this.whiteHighlightMaterial = whiteHighlightMaterial;
        this.blackKillMaterial = blackKillMaterial;
        this.whiteKillMaterial = whiteKillMaterial;
    }   
}

public class AppearanceManager3D : MonoBehaviour
{
    [Header("Settings")]

    public ThemePack[] themes;

    public int DefaultTheme;

    [HideInInspector]
    public int ActiveTheme;

    [Header("References")]
    public AppearanceManager2D appearanceManager2D;
    public Chessboard3DPieceManager pieceManager;

    public GameObject FixedParent;
    public GameObject BoardParent;
    public GameObject WhiteSquaresParent;
    public GameObject BlackSquaresParent;

    public ChessManagerInterface chessManagerInterface;

    // Internal
    [HideInInspector]
    public Chessboard3DSquare[,] Board = new Chessboard3DSquare[8, 8];

    [HideInInspector]
    public Vector2Int hoveringOver = new Vector2Int(-1, -1);

    [HideInInspector]
    public float[,] target_displacement = new float[8, 8];
    [HideInInspector]
    public int[,] highlighted = new int[8, 8];

    [HideInInspector]
    public List<RippleData> ripples = new List<RippleData>();

    private void Awake()
    {
        ActiveTheme = DefaultTheme;
    }

    void Start()
    {
        ChangeTheme(DefaultTheme);
    }

    public void ChangeTheme(int themeID)
    {
        ActiveTheme = themeID;
        for (int i = 0; i < WhiteSquaresParent.transform.childCount; i++)
        {
            WhiteSquaresParent.transform.GetChild(i).GetComponent<Chessboard3DSquare>().SetMaterial(themes[themeID].whiteSquareMaterial, 
                themes[themeID].whiteHighlightMaterial, themes[themeID].whiteKillMaterial);
        }

        for (int i = 0; i < BlackSquaresParent.transform.childCount; i++)
        {
            BlackSquaresParent.transform.GetChild(i).GetComponent<Chessboard3DSquare>().SetMaterial(themes[themeID].blackSquareMaterial, 
                themes[themeID].blackHighlightMaterial, themes[themeID].blackKillMaterial);
        }

        pieceManager.UpdateTheme();

        appearanceManager2D.ChangeTheme(themes[themeID].whiteSquareMaterial, themes[themeID].blackSquareMaterial);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            int new_theme = ActiveTheme + 1;
            if (new_theme >= themes.Length)
            {
                new_theme = 0;
            }
            ChangeTheme(new_theme);
        }

        target_displacement = new float[8, 8];
        highlighted = new int[8, 8];

        if (hoveringOver.x != -1)
        {
            target_displacement[hoveringOver.x, hoveringOver.y] += 0.2f;
        }

        if (chessManagerInterface.Selected.x != -1)
        {
            target_displacement[chessManagerInterface.Selected.x, chessManagerInterface.Selected.y] += 0.3f;
        }

        foreach (Tuple<Vector2Int, bool> possible_move in chessManagerInterface.possibleMoves)
        {
            target_displacement[possible_move.Item1.x, possible_move.Item1.y] += 0.2f;
            if (chessManagerInterface.chessManager.State.GetPieceAtPosition(possible_move.Item1) != null)
            {
                highlighted[possible_move.Item1.x, possible_move.Item1.y] = 2;
            }
            else
            {
                highlighted[possible_move.Item1.x, possible_move.Item1.y] = 1;
            }
        }

        int i = 0;
        while (i < ripples.Count)
        {
            if (ripples[i].frame_num >= Ripple.FRAME_COUNT)
            {
                ripples.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        foreach (RippleData ripple in ripples)
        {
            float[,] frame = ripple.GetFrame();

            for (int x = 0; x < 8; x ++)
            {
                for (int y = 0; y < 8; y++)
                {
                    target_displacement[x, y] += frame[x, y];
                }
            }
        }
    }
}
