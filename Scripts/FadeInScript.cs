using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInScript : MonoBehaviour
{
    public Image Image;
    public float time;
    float progress;

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime / time;

        if (progress > 1) { Destroy(gameObject); return; }

        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 1 - progress);
    }
}
