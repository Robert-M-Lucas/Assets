using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] PieceMoveSounds;
    public AudioClip OnClick;
    public AudioClip OffClick;

    [Header("References")]
    public Transform CameraPos;
    public GameObject Soundling;

    int _previous = -1;
    public void PlayOnClick()
    {
        // PlaySound(OnClick);
    }

    public void PlayOffClick()
    {
        // PlaySound(OffClick);
    }

    public void PlayPieceMoveSound(Vector3 pos)
    {
        int random = Random.Range(0, PieceMoveSounds.Length - 1);
        if (random == _previous)
        {
            random += 1;
            if (random >= PieceMoveSounds.Length) { random = 0; }
        }
        _previous = random;
        PlaySound(PieceMoveSounds[random], pos);
    }

    public void PlaySound(AudioClip clip, Vector3? pos = null)
    {
        GameObject soundling = Instantiate(Soundling);
        AudioSource audioSource = soundling.GetComponent<AudioSource>();
        audioSource.clip = clip;
        soundling.transform.position = pos ?? CameraPos.position;
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
