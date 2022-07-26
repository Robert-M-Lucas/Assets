using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOutScript : MonoBehaviour
{
    public Image Image;
    public float time;
    float progress;

    public bool Fade = false;

    // Update is called once per frame
    void Update()
    {
        if (Fade)
        {
            progress += Time.deltaTime / time;

            if (progress > 1) { SceneManager.LoadScene(0); return; }

            Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, progress);
        }
    }
}
