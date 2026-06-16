using UnityEngine;

public class BugCollector : MonoBehaviour
{
    // Sound played when the bug is collected
    public AudioClip collectSound;

    // Tracks whether the bug has already been collected
    public bool isCollected = false;

    void OnCollisionEnter(Collision other)
    {
        // Check if the player collected the bug
        if (other.gameObject.CompareTag("Player"))
        {
            AudioSource audio = Camera.main.GetComponent<AudioSource>();
            audio.PlayOneShot(collectSound, 1f);

            GameManager.instance.CollectBug(this);
        }
    }
}