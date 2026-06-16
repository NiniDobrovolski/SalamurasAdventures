[System.Serializable]
public class Question
{
    public string questionText;
    public string correctAnswer;
    public QuestionState state = QuestionState.Pending;
}