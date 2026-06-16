using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;
    public Image progressBar;

    private int notesCollected = 0;
    private int totalNotes = 20;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            GameFlow.Instance.ReachEnd();
        }
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (progressBar != null)
            progressBar.fillAmount = 0f;
    }

    public void CollectNote()
    {
        notesCollected++;

        // Update progress bar
        if (progressBar != null && totalNotes > 0)
        {
            progressBar.fillAmount = (float)notesCollected / totalNotes;
        }

        // Trigger level completion when all notes are collected
        if (notesCollected >= totalNotes)
        {
            GameFlow.Instance.ReachEnd();
        }
    }

    public void AddNote()
    {
        // Reserved for dynamic note scaling (currently disabled)
        // totalNotes++;
    }
}