using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject questionPanel;

    public void PlayGame()
    {
        // Initialize questions if available before starting the game
        if (QuestionManager.Instance.allQuestions.Count > 0)
        {
            QuestionManager.Instance.currentLevel = 1;
            QuestionManager.Instance.RecalculateDistribution(true);
        }

        SceneManager.LoadScene("Game Level Scenes/Cut Scene 1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenQuestionUI()
    {
        questionPanel.SetActive(true);
    }
}