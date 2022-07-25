using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text theText;
    public Color TextHoverColor;
    Color defaultColor;

    public TMP_Text textSizeSource;

    public void Awake()
    {
        defaultColor = theText.color;
    }

    public void Start()
    {
        if (textSizeSource is not null)
        {
            theText.fontSize = textSizeSource.fontSize;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = TextHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = defaultColor;
    }

    public void Update()
    {
        if (textSizeSource is not null)
        {
            theText.fontSize = textSizeSource.fontSize;
        }
    }
}
