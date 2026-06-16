using UnityEngine;

[System.Serializable]
public class FluteHole
{
    public GameObject hole;     // Hole object
    public KeyCode key;         // Assigned key
    public Color pressedColor;  // Color when pressed
    public AudioClip note;      // Sound played on press

    public static bool IsOpen = false;
}

public class SalamuriHoles : MonoBehaviour
{
    public FluteHole[] holes;

    public Color normalColor = Color.black;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Disable interaction while question UI is open
        if (QuestionUI.IsOpen) return;

        foreach (FluteHole h in holes)
        {
            Renderer r = h.hole.GetComponent<Renderer>();

            if (Input.GetKeyDown(h.key))
            {
                r.material.color = h.pressedColor;

                if (h.note != null)
                    audioSource.PlayOneShot(h.note);
            }

            if (Input.GetKeyUp(h.key))
            {
                r.material.color = normalColor;
            }
        }
    }
}