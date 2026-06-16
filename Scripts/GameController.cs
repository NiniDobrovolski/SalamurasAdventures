using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TMP_Text flipText;
    public TMP_Text noteText;

    [Header("Progress Bar")]
    public Image progressBar;
    private int currentProgress = 0;
    private int maxProgress = 40;

    private bool flipActive = false;
    private bool noteActive = false;

    private bool flipDone = false;
    private bool noteDone = false;

    private string requiredFlip = "";
    private KeyCode currentNoteKey;

    private float taskDuration = 2f;

    public Animator animator;

    [Header("Flip UI")]
    public GameObject flipBackground;

    [Header("Note UI")]
    [Header("Game Over Setup")]
    public Collider ropeCollider;
    public Rigidbody playerRigidbody;
    public GameObject noteBackground;

    private bool isGameOver = false;

    private int flipTaskCount = 0;
    private int tutorialFlipCount = 10;

    struct InputEvent
    {
        public KeyCode key;
        public float time;
    }

    List<InputEvent> inputBuffer = new List<InputEvent>();
    float inputBufferTime = 0.15f;

    void Start()
    {
        flipBackground.SetActive(false);
        noteBackground.SetActive(false);
        flipText.text = "";
        noteText.text = "";
        progressBar.fillAmount = 0f;

        StartCoroutine(FlipRoutine());
        StartCoroutine(NoteRoutine());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            QuestionManager.Instance.currentLevel++;
            QuestionManager.Instance.RecalculateDistribution(false);
            SceneManager.LoadScene("Cut Scene 4");
        }

        if (isGameOver) { return; }
        if (QuestionUI.IsOpen) return;

        CaptureInput();
        ProcessInputBuffer();
        CleanBuffer();
    }

    // Captures single key press into buffer
    void CaptureInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    inputBuffer.Add(new InputEvent { key = key, time = Time.time });
                    break;
                }
            }
        }
    }

    void ProcessInputBuffer()
    {
        for (int i = 0; i < inputBuffer.Count; i++)
        {
            var input = inputBuffer[i];

            // Handle flip task input
            if (flipActive && !flipDone)
            {
                if (IsFlipKey(input.key))
                {
                    string pressed = input.key.ToString().Replace("Alpha", "");

                    if (pressed == requiredFlip)
                    {
                        flipDone = true;
                        flipText.text = "";

                        TriggerFlipAnimation(input.key);
                        AddProgress(1);

                        flipBackground.SetActive(false);
                    }
                    else
                    {
                        GameOver();
                        return;
                    }
                }
            }

            // Handle note task input
            if (noteActive && !noteDone)
            {
                if (IsNoteKey(input.key))
                {
                    if (input.key == currentNoteKey)
                    {
                        noteDone = true;
                        noteText.text = "";

                        AddProgress(1);

                        noteBackground.SetActive(false);
                    }
                    else
                    {
                        GameOver();
                        return;
                    }
                }
            }
        }

        inputBuffer.Clear();
    }

    void CleanBuffer()
    {
        inputBuffer.RemoveAll(i => Time.time - i.time > inputBufferTime);
    }

    // Continuously spawns flip tasks
    IEnumerator FlipRoutine()
    {
        while (true)
        {
            if (isGameOver) yield break;

            yield return new WaitForSeconds(Random.Range(0f, 0.2f));

            if (!flipActive)
            {
                StartCoroutine(FlipTask());
            }
        }
    }

    // Continuously spawns note tasks
    IEnumerator NoteRoutine()
    {
        while (true)
        {
            if (isGameOver) yield break;

            yield return new WaitForSeconds(Random.Range(0f, 0f));

            if (!noteActive)
            {
                StartCoroutine(NoteTask());
            }
        }
    }

    IEnumerator FlipTask()
    {
        if (isGameOver) yield break;

        flipActive = true;
        flipDone = false;

        int rand = Random.Range(1, 5);
        requiredFlip = rand.ToString();

        flipTaskCount++;

        string flipName = GetFlipName(requiredFlip);

        if (flipTaskCount <= tutorialFlipCount)
        {
            flipText.text = flipName + " [" + requiredFlip + "]";
        }
        else
        {
            flipText.text = flipName;
        }

        flipBackground.SetActive(true);

        float elapsed = 0f;

        while (elapsed < taskDuration)
        {
            if (isGameOver) yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isGameOver) yield break;

        if (!flipDone)
        {
            GameOver();
        }

        flipBackground.SetActive(false);
        flipActive = false;
    }

    string GetFlipName(string flipNumber)
    {
        switch (flipNumber)
        {
            case "1": return "ერთმაგი მალაყი";
            case "2": return "ორმაგი მალაყი";
            case "3": return "სამმაგი მალაყი";
            case "4": return "უკანა მალაყი";
            default: return "";
        }
    }

    IEnumerator NoteTask()
    {
        if (isGameOver) yield break;

        noteActive = true;
        noteDone = false;

        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0: currentNoteKey = KeyCode.B; noteText.text = "დაუკარი B"; break;
            case 1: currentNoteKey = KeyCode.D; noteText.text = "დაუკარი D"; break;
            case 2: currentNoteKey = KeyCode.E; noteText.text = "დაუკარი E"; break;
            case 3: currentNoteKey = KeyCode.G; noteText.text = "დაუკარი G"; break;
        }

        noteBackground.SetActive(true);

        float elapsed = 0f;

        while (elapsed < taskDuration)
        {
            if (isGameOver) yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isGameOver) yield break;

        if (!noteDone)
        {
            GameOver();
        }

        noteBackground.SetActive(false);
        noteActive = false;
    }

    // Checks if key belongs to flip input set
    bool IsFlipKey(KeyCode key)
    {
        return key == KeyCode.Alpha1 ||
               key == KeyCode.Alpha2 ||
               key == KeyCode.Alpha3 ||
               key == KeyCode.Alpha4;
    }

    // Checks if key belongs to note input set
    bool IsNoteKey(KeyCode key)
    {
        return key == KeyCode.B ||
               key == KeyCode.D ||
               key == KeyCode.E ||
               key == KeyCode.G;
    }

    void TriggerFlipAnimation(KeyCode key)
    {
        if (key == KeyCode.Alpha1) animator.SetTrigger("Running Forward");
        else if (key == KeyCode.Alpha2) animator.SetTrigger("Double Flip");
        else if (key == KeyCode.Alpha3) animator.SetTrigger("Triple Flip");
        else if (key == KeyCode.Alpha4) animator.SetTrigger("Backflip");
    }

    void AddProgress(int amount)
    {
        currentProgress += amount;
        currentProgress = Mathf.Clamp(currentProgress, 0, maxProgress);
        progressBar.fillAmount = (float)currentProgress / maxProgress;

        if (currentProgress >= maxProgress && !isGameOver)
        {
            QuestionManager.Instance.currentLevel++;
            QuestionManager.Instance.RecalculateDistribution(false);
            SceneManager.LoadScene("Cut Scene 4");
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("Game Over!");

        // Disable gameplay states immediately
        flipActive = false;
        noteActive = false;

        // Hide UI
        flipBackground.SetActive(false);
        noteBackground.SetActive(false);

        flipText.text = "";
        noteText.text = "";

        progressBar.fillAmount = 0f;
        currentProgress = 0;

        inputBuffer.Clear();

        // Enable ragdoll / physics
        if (ropeCollider != null)
            ropeCollider.enabled = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            playerRigidbody.useGravity = true;
        }

        animator.SetTrigger("Fall");

        StartCoroutine(RestartAfterFall());
    }

    IEnumerator RestartAfterFall()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}