using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    public int currentLevel = 1;

    public List<Question> allQuestions = new List<Question>();

    public List<Question> level1Questions = new List<Question>();
    public List<Question> level2Questions = new List<Question>();
    public List<Question> level3Questions = new List<Question>();
    public List<Question> level4Questions = new List<Question>();

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetQuestions(List<Question> questions)
    {
        // Replace question pool and reset states
        allQuestions = new List<Question>(questions);

        foreach (var q in allQuestions)
            q.state = QuestionState.Pending;
    }

    public void MarkAnswered(Question q)
    {
        q.state = QuestionState.Answered;
    }

    public List<Question> GetAvailableQuestions()
    {
        // Return only unanswered questions
        return allQuestions
            .Where(q => q.state == QuestionState.Pending)
            .ToList();
    }

    public void RecalculateDistribution(bool menu)
    {
        // Reset all questions when coming from menu
        if (menu)
        {
            foreach (var q in allQuestions)
                q.state = QuestionState.Pending;
        }

        var remaining = GetAvailableQuestions();
        Shuffle(remaining);

        level1Questions.Clear();
        level2Questions.Clear();
        level3Questions.Clear();
        level4Questions.Clear();

        int total = remaining.Count;
        if (total == 0) return;

        // Base level distribution ratios
        float p1 = 0.5f;
        float p2 = 0.15f;
        float p3 = 0.12f;
        float p4 = 1f - (p1 + p2 + p3);

        // Normalize only for remaining active levels
        float remainingPercent = 0f;

        if (currentLevel <= 1) remainingPercent += p1;
        if (currentLevel <= 2) remainingPercent += p2;
        if (currentLevel <= 3) remainingPercent += p3;
        if (currentLevel <= 4) remainingPercent += p4;

        int index = 0;

        if (currentLevel <= 1)
        {
            int count = Mathf.RoundToInt(total * (p1 / remainingPercent));
            level1Questions = remaining.GetRange(index, Mathf.Min(count, total - index));
            index += level1Questions.Count;
        }

        if (currentLevel <= 2)
        {
            int count = Mathf.RoundToInt(total * (p2 / remainingPercent));
            level2Questions = remaining.GetRange(index, Mathf.Min(count, total - index));
            index += level2Questions.Count;
        }

        if (currentLevel <= 3)
        {
            int count = Mathf.RoundToInt(total * (p3 / remainingPercent));
            level3Questions = remaining.GetRange(index, Mathf.Min(count, total - index));
            index += level3Questions.Count;
        }

        if (currentLevel <= 4)
        {
            level4Questions = remaining.GetRange(index, Mathf.Max(0, total - index));
        }
    }

    // Fisher-Yates shuffle
    void Shuffle(List<Question> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}