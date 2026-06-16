using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestionUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text questionText;
    public TMP_InputField answerField;
    public Image answerBox;

    public static bool IsOpen = false;

    private Question currentQuestion;
    private System.Action onCorrect;

    void Start()
    {
        // Listen for TMP input submit event
        answerField.onSubmit.AddListener(HandleSubmit);
    }

    void HandleSubmit(string text)
    {
        if (IsOpen)
            Submit();
    }

    public void OnTyping()
    {
        // Reset answer box color when user starts typing
        answerBox.color = Color.white;
    }

    public void ShowQuestion(Question q, System.Action correctCallback)
    {
        // Pause game while question is active
        Time.timeScale = 0f;
        IsOpen = true;

        currentQuestion = q;
        onCorrect = correctCallback;

        panel.SetActive(true);
        questionText.text = q.questionText;
        answerField.text = "";

        // Focus input field automatically
        EventSystem.current.SetSelectedGameObject(answerField.gameObject);
        answerField.ActivateInputField();
    }

    public void Submit()
    {
        // Compare normalized answers (case + spacing insensitive)
        if (answerField.text.Trim().ToLower() ==
            currentQuestion.correctAnswer.Trim().ToLower())
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
            IsOpen = false;

            onCorrect?.Invoke();
        }
        else
        {
            // Wrong answer feedback
            answerBox.color = Color.red;
        }
    }

    void Correct()
    {
        panel.SetActive(false);
        answerBox.color = Color.white;

        GamePause.Resume();

        onCorrect?.Invoke();
    }

    public static class GamePause
    {
        public static void Pause()
        {
            Time.timeScale = 0f;
        }

        public static void Resume()
        {
            Time.timeScale = 1f;
        }
    }
}