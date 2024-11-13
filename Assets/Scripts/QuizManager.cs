using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public int totalQuestions;
    public GameObject[] options;
    public Text QuestionTxt;
    public ScoreScript score;

    private List<QnA> QnAs;
    private List<int> displayedQuestions = new List<int>();
    private int currentQuestion;

    private int correctAnswerCount = 0;

    public event Action<int, int> OnQuizEnd;


    public void LoadQuiz(SectionData section, LevelData level)
    {
        displayedQuestions.Clear();
        score.score = 0;
        //generate quiz
        if (section != null && level != null)
        {
            QnAs = level.QnAs;
            totalQuestions = QnAs.Count;

            Debug.Log($"IsQnAs null? : {QnAs}");
            GenerateQuestion();
        }
    }

    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(1);
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        Debug.Log("Generated");
        if (!(totalQuestions == 0 || totalQuestions == displayedQuestions.Count))
        {
            do
            {
                currentQuestion = UnityEngine.Random.Range(0, totalQuestions);
            } while (displayedQuestions.Contains(currentQuestion));
            displayedQuestions.Add(currentQuestion);

            QuestionTxt.text = QnAs[currentQuestion].Question; //??
            SetAnswers(); //generate options
        }
        else
        {
            Debug.Log("Out of Questions");
            Debug.Log($"Jumlah pertanyaan: {displayedQuestions.Count}");
            AchievementManager.Instance.CheckAchievementProgress(correctAnswerCount);
            OnQuizEnd?.Invoke(score.GetScore(), totalQuestions); //event soal habis
        }
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<Image>().color = options[i].GetComponent<AnswerScript>().startColor;
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnAs[currentQuestion].Answers[i];

            // ArabicText arabicTextScript = options[i].transform.GetChild(0).GetComponent<ArabicText>();
            // arabicTextScript.Text = QnAs[currentQuestion].Answers[i];
            // arabicTextScript.RefreshText();

            if (QnAs[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    public void Correct()
    {
        score.AddScore();
        correctAnswerCount++;
        StartCoroutine(WaitForNext());
    }

    public void Wrong()
    {
        StartCoroutine(WaitForNext());
    }
}
