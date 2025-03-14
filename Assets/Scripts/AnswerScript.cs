using System;
using UnityEngine;
using UnityEngine.UI;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    public Color startColor;
    public QuizManager quizManager;
    public AudioManager audio;

    private void Start()
    {
        startColor = GetComponent<Image>().color;
    }

    public void Answer()
    {
        if (isCorrect)
        {
            GetComponent<Image>().color = quizManager.correctColor;
            Debug.Log("Correct Answer");
            quizManager.Correct();

            audio.PlaySFX(audio.right);
        }
        else
        {
            GetComponent<Image>().color = quizManager.wrongColor;
            Debug.Log("Wrong Answer");
            quizManager.Wrong();

            audio.PlaySFX(audio.wrong);
        }
    }
}
