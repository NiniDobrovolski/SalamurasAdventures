using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionInputUI : MonoBehaviour
{
    public GameObject questionRowPrefab;
    public RectTransform contentParent;
    public ScrollRect scrollRect;

    public GameObject questionPanel;

    private List<GameObject> rows = new List<GameObject>();

    void Start()
    {
        // Load saved questions from manager into UI
        LoadFromManager();

        // Cache existing rows in the content container
        foreach (Transform child in contentParent)
            rows.Add(child.gameObject);

        Debug.Log("Initial rows count: " + rows.Count);
    }

    void CreateNewRow()
    {
        // Limit max number of rows
        if (rows.Count >= 50) return;

        // Instantiate a new question row UI
        GameObject row = Instantiate(questionRowPrefab, contentParent);
        rows.Add(row);

        QuestionRowUI rowUI = row.GetComponent<QuestionRowUI>();

        // Setup add button for new row
        rowUI.addButton.onClick.RemoveAllListeners();
        rowUI.addButton.onClick.AddListener(AddRow);

        rowUI.deleteButton.gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void AddRow()
    {
        // If no rows exist, create first one
        if (rows.Count == 0)
        {
            CreateNewRow();
            return;
        }

        GameObject currentRow = rows[rows.Count - 1];
        QuestionRowUI rowUI = currentRow.GetComponent<QuestionRowUI>();

        string question = rowUI.questionInput.text;
        string answer = rowUI.answerInput.text;

        // Ignore empty inputs
        if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            return;

        // Lock current row UI state
        rowUI.addButton.gameObject.SetActive(false);
        rowUI.deleteButton.gameObject.SetActive(true);

        // Assign delete action for this row
        rowUI.deleteButton.onClick.RemoveAllListeners();
        rowUI.deleteButton.onClick.AddListener(() => DeleteRow(currentRow));

        // Create next empty row
        GameObject newRow = Instantiate(questionRowPrefab, contentParent);
        rows.Add(newRow);

        QuestionRowUI newRowUI = newRow.GetComponent<QuestionRowUI>();

        newRowUI.addButton.onClick.RemoveAllListeners();
        newRowUI.addButton.onClick.AddListener(AddRow);

        newRowUI.questionInput.text = "";
        newRowUI.answerInput.text = "";

        newRowUI.addButton.gameObject.SetActive(true);
        newRowUI.deleteButton.gameObject.SetActive(false);

        // Scroll UI to bottom after adding row
        StartCoroutine(ScrollToBottom());
    }

    public void DeleteRow(GameObject row)
    {
        // Remove row from list and destroy it
        if (rows.Contains(row))
        {
            rows.Remove(row);
            Destroy(row);
        }
    }

    IEnumerator ScrollToBottom()
    {
        // Wait one frame for UI layout update
        yield return null;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void StartGame()
    {
        // Clear previous questions
        QuestionManager.Instance.allQuestions.Clear();

        // Convert UI rows into Question objects
        foreach (GameObject row in rows)
        {
            var inputs = row.GetComponentsInChildren<TMP_InputField>();

            string question = inputs[0].text;
            string answer = inputs[1].text;

            if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
            {
                QuestionManager.Instance.allQuestions.Add(new Question
                {
                    questionText = question,
                    correctAnswer = answer
                });
            }
        }

        // Recalculate distribution if questions exist
        if (QuestionManager.Instance.allQuestions.Count > 0)
            QuestionManager.Instance.RecalculateDistribution(false);

        // Start game scene
        SceneManager.LoadScene("Level1");
    }

    public void SaveToManager()
    {
        List<Question> list = new List<Question>();

        // Save UI rows into manager list
        foreach (GameObject row in rows)
        {
            var inputs = row.GetComponentsInChildren<TMP_InputField>();

            string question = inputs[0].text;
            string answer = inputs[1].text;

            if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
            {
                list.Add(new Question
                {
                    questionText = question,
                    correctAnswer = answer
                });
            }
        }

        QuestionManager.Instance.SetQuestions(list);
        Debug.Log("Saved to manager!");
    }

    public void ClosePanel()
    {
        // Save before closing UI panel
        SaveToManager();
        questionPanel.SetActive(false);
    }

    void LoadFromManager()
    {
        // Load existing questions into UI
        foreach (var q in QuestionManager.Instance.allQuestions)
        {
            GameObject row = Instantiate(questionRowPrefab, contentParent);
            rows.Add(row);

            var ui = row.GetComponent<QuestionRowUI>();
            ui.questionInput.text = q.questionText;
            ui.answerInput.text = q.correctAnswer;

            ui.addButton.onClick.RemoveAllListeners();
            ui.addButton.onClick.AddListener(AddRow);

            ui.deleteButton.gameObject.SetActive(true);
            ui.addButton.gameObject.SetActive(false);

            ui.deleteButton.onClick.AddListener(() => DeleteRow(row));
        }
    }
}