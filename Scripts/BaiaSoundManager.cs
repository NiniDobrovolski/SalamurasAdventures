using UnityEngine;
using System.Collections;

public class BaiaSoundManager : MonoBehaviour
{
    // Audio source used to play the sound
    public AudioSource audioSource;

    void Start()
    {
        StartCoroutine(PlaySoundLoop());
    }

    // Plays the sound repeatedly after an initial delay
    IEnumerator PlaySoundLoop()
    {
        yield return new WaitForSeconds(30f);

        while (true)
        {
            audioSource.Play();

            // Wait before playing the sound again
            yield return new WaitForSeconds(40f);
        }
    }
}