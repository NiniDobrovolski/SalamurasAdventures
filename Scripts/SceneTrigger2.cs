using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger2 : MonoBehaviour
{
    public string sceneToLoad = "Game Level Scenes/Cut Scene 3";

    private void OnTriggerEnter(Collider other)
    {
        // Trigger scene change when player enters collider
        if (other.CompareTag("Player"))
        {
            QuestionManager.Instance.currentLevel++;
            QuestionManager.Instance.RecalculateDistribution(false);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}