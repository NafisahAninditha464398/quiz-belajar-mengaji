using UnityEngine;
[CreateAssetMenu(fileName = "New QnA", menuName = "Quiz/QnA")]
public class QnA : ScriptableObject
{
    public string Question;
    public string QuestionDesc;
    // public string[] Answers;
    public Sprite[] Answers;
    public int CorrectAnswer;
}