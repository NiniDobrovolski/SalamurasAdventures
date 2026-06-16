using UnityEngine;

public class Trap : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Cache AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Play trap sound when player enters trigger
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
        }
    }
}