using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelQuestionScheduler : MonoBehaviour
{
    public float levelDurationMinutes = 5f;
    private float levelDurationSeconds;
    private float interval;

    private Queue<Question> questions;

    public QuestionUI questionUI;

    private List<Question> sessionQuestions = new List<Question>();

    void Start()
    {
        SetLevelDuration();

        levelDurationSeconds = levelDurationMinutes * 60f;

        questions = GetQuestionsForScene();

        if (questions.Count == 0) return;

        interval = levelDurationSeconds / (questions.Count + 1);

        StartCoroutine(RunQuestions());
    }

    // Sets level duration based on current scene
    void SetLevelDuration()
    {
        string scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Scene 1": levelDurationMinutes = 5f; break;
            case "Scene 2": levelDurationMinutes = 0.83f; break;
            case "Scene 3": levelDurationMinutes = 0.67f; break;
            case "Scene 4": levelDurationMinutes = 2f; break;
        }
    }

    // Retrieves question set for current scene
    Queue<Question> GetQuestionsForScene()
    {
        string scene = SceneManager.GetActiveScene().name;

        List<Question> list = scene switch
        {
            "Scene 1" => QuestionManager.Instance.level1Questions,
            "Scene 2" => QuestionManager.Instance.level2Questions,
            "Scene 3" => QuestionManager.Instance.level3Questions,
            "Scene 4" => QuestionManager.Instance.level4Questions,
            _ => new List<Question>()
        };

        return new Queue<Question>(list);
    }

    IEnumerator RunQuestions()
    {
        while (questions.Count > 0)
        {
            float timer = 0f;
            bool answered = false;

            // Wait for interval or until answered
            while (timer < interval && !answered)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Question q = questions.Dequeue();
            sessionQuestions.Add(q);

            // Show question and handle answer callback
            questionUI.ShowQuestion(q, () =>
            {
                answered = true;
                q.state = QuestionState.Answered;

                QuestionManager.Instance.MarkAnswered(q);
            });

            // Wait until answered or timeout ends
            while (!answered && timer < interval)
            {
                yield return null;
            }
        }

        // End of level → redistribute remaining questions
        QuestionManager.Instance.RecalculateDistribution(false);
    }
}