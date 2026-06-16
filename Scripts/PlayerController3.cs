using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public TileManager tileManager;

    public float dropForce = 25f;
    private Rigidbody rb;
    private bool isGrounded = false;
    private GameObject currentTile;

    private int expectedNextIndex = 0;
    private GameObject currentObstacle;

    private float startTime = 300;
    private float timeRemaining;
    private bool gameEnded = false;

    private Renderer playerRenderer;
    private bool isBlinking = false;

    public Image timerFill;
    public TextMeshProUGUI timerText;
    private Color color;

    private AudioSource audioSource;
    Animator animator;

    public string sceneToLoad = "Game Level Scenes/Cut Scene 5";

    private int tileCounter = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerRenderer = GetComponentInChildren<Renderer>();

        timeRemaining = GameData.savedTime > 0 ? GameData.savedTime : startTime;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        color = timerFill.color;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            gameEnded = true;
            SceneManager.LoadScene(sceneToLoad);
            Time.timeScale = 0f;
        }
        if (gameEnded)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            GameOver();
        }

        // Drop through tile / obstacle
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && isGrounded)
        {
            isGrounded = false;

            if (currentTile != null)
            {
                currentTile.GetComponent<Collider>().enabled = false;
            }

            if (currentObstacle != null)
            {
                currentObstacle.GetComponent<Collider>().enabled = false;
            }

            rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
            animator.SetBool("IsFalling", true);
        }

        timerFill.fillAmount = timeRemaining / startTime;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerFill.color = timeRemaining <= 50f ? Color.red : color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            tileCounter++;

            GameObject newTile = collision.gameObject;
            if (newTile == currentTile)
                return;

            MovingTile tileComponent = newTile.GetComponent<MovingTile>();
            if (tileComponent == null)
                return;

            int newIndex = tileComponent.tileID;

            if (expectedNextIndex != -1 && newIndex != expectedNextIndex)
            {
                GameData.savedTime = timeRemaining;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            if (currentTile != null)
            {
                MovingTile oldTile = currentTile.GetComponent<MovingTile>();
                if (oldTile != null)
                    oldTile.grounded = false;
            }

            currentTile = newTile;

            tileComponent.grounded = true;
            isGrounded = true;
            animator.SetBool("IsFalling", false);

            expectedNextIndex = newIndex + 1;

            tileManager.CheckAndSpawn(currentTile);
        }

        if (collision.gameObject.CompareTag("WinGame"))
        {
            if (tileCounter == 61)
            {
                gameEnded = true;
                SceneManager.LoadScene(sceneToLoad);
                Time.timeScale = 0f;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            currentTile = collision.gameObject;
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Trap"))
        {
            StartCoroutine(BlinkEffect());
            ApplyPenalty(10f);
        }

        if (other.CompareTag("Rat"))
        {
            StartCoroutine(BlinkEffect());
            ApplyPenalty(5f);
        }

        if (other.CompareTag("Juice"))
        {
            timeRemaining = Mathf.Min(timeRemaining + 5f, 300f);
            audioSource.Play();
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Spike"))
        {
            StartCoroutine(BlinkEffect());
            ApplyPenalty(7f);
        }
    }

    void ApplyPenalty(float amount)
    {
        timeRemaining = Mathf.Max(timeRemaining - amount, 0f);

        if (timeRemaining <= 0f)
            GameOver();
    }

    void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        timeRemaining = 0f;
        GameData.savedTime = 0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator BlinkEffect()
    {
        if (isBlinking)
            yield break;

        isBlinking = true;

        float blinkDuration = 1f;
        float blinkInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            playerRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            playerRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2;
        }

        playerRenderer.enabled = true;
        isBlinking = false;
    }
}