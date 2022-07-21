using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceManager2D : MonoBehaviour
{
    [Header("Settings")]
    public float target_opacity = 0.75f;

    [Header("References")]
    public CanvasGroup canvasGroup;
    public CameraControlScript cameraControl;
    public GameObject Parent;

    public Transform WhiteSquaresParent;
    public Transform BlackSquaresParent;

    public void ChangeTheme(Material White, Material Black)
    {
        for (int i = 0; i < WhiteSquaresParent.transform.childCount; i++)
        {
            WhiteSquaresParent.transform.GetChild(i).GetComponent<Image>().color = White.color;
        }

        for (int i = 0; i < BlackSquaresParent.transform.childCount; i++)
        {
            BlackSquaresParent.transform.GetChild(i).GetComponent<Image>().color = Black.color;
        }
    }

    void Update()
    {
        if (cameraControl.perspective_progress < 0.7f)
        {
            canvasGroup.alpha = 0;
        }
        else
        {
            canvasGroup.alpha = MathP.CosSmooth((cameraControl.perspective_progress - 0.7f) * (1/0.3f)) * target_opacity;
        }

        if (cameraControl.perspective_progress == 0 && Parent.activeInHierarchy)
        {
            Parent.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        else if (cameraControl.perspective_progress != 0 && !Parent.activeInHierarchy)
        {
            Parent.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }
}
