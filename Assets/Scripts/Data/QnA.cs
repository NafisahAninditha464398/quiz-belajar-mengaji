using UnityEngine;
[CreateAssetMenu(fileName = "New QnA", menuName = "Quiz/QnA")]
public class QnA : ScriptableObject
{
    public string Question;
    public string QuestionDesc;
    public string[] Answers;
    public int CorrectAnswer;
}