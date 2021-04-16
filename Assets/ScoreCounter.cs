using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public int score;
    int calScore;
    public TextMeshProUGUI scoreText;

    public Image scoreImage;

    private void Start()
    {
        calScore = score;
    }
    public void CalculateScore()
    {
        scoreImage.gameObject.SetActive(true);
        for (int i = 0; i < score - 1; i++)
        {
            calScore++;
        }
        if (calScore >= score)
        {
            calScore = score;
        }
        scoreText.text = "" + calScore;
    }
}
