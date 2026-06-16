using UnityEngine;

public class Spike3 : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Cache AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Play spike sound when player enters trigger
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
        }
    }
}