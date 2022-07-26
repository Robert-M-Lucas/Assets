using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public GameObject Soundling;
    public AudioClip OnClick;

    public void PlayOnClick()
    {
        PlaySound(OnClick);
    }

    public void PlaySound(AudioClip clip, Vector3? pos = null)
    {
        GameObject soundling = Instantiate(Soundling);
        AudioSource audioSource = soundling.GetComponent<AudioSource>();
        audioSource.clip = clip;
        soundling.transform.position = pos ?? Camera.main.transform.position;
        audioSource.Play();
        DontDestroyOnLoad(soundling);
        StartCoroutine(DeleteSoundling(soundling, clip.length));
    }

    IEnumerator DeleteSoundling(GameObject soundling, float delay)
    {
        yield return new WaitForSeconds(delay+1);
        Destroy(soundling);
    }
}
