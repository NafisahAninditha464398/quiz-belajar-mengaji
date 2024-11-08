using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public int score;
    public int Stars = 0;
    
    //memanggil nilai score
    public int GetScore()
    {
        return score;
    }

    //memanggil jumlah bintang
    public int GetStars()
    {
        return Stars;
    }

    //menambah score
    public void AddScore()
    {
        score++;
    }

    //menghitung jumlah bintang
    public void CountStars(int totalQuestions)
    {
        if (score == 0)
        {
            Stars = 0;
        }
        else
        {
            float ratio = (float)score / (float)totalQuestions;
            if (ratio == 1) //100%
            {
                Stars = 3;
            }
            else if (ratio >= 3f / 4f) //75%
            {
                Stars = 2;
            }
            else if (ratio >= 2f / 4f) //50%
            {
                Stars = 1;
            }
            else if (ratio >= 1f / 4f) //25%
            {
                Stars = 0;
            }
            // Debug.Log("ratio: " + ratio);
            // Debug.Log("score: " + score);
            // Debug.Log("tot: " + totalQuestions);
        }

    }
}
