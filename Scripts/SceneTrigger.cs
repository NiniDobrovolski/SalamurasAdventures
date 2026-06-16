using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad = "Game Level Scenes/Cut Scene 2";

    private bool cheatUnlocked = false;

    void Update()
    {
        // Cheat: unlock scene transition instantly
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
        {
            cheatUnlocked = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trigger only when player collides with object
        if (collision.gameObject.CompareTag("Player"))
        {
            // Allow progression if all bugs collected or cheat enabled
            if (GameManager.instance.collectedBugs.Count == 20 || cheatUnlocked)
            {
                Time.timeScale = 1f;

                Destroy(GameManager.instance.gameObject);

                QuestionManager.Instance.currentLevel++;
                QuestionManager.Instance.RecalculateDistribution(false);

                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}