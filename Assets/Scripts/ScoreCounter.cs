using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreCounter : MonoBehaviour
{
    public int score;
    private int calScore;
    public TextMeshProUGUI scoreText;
    public Image scoreImage;

    private void Start()
    {
        calScore = score;
    }

    public void CalculateScore()
    {
        scoreImage.gameObject.SetActive(true);
        scoreImage.transform.DOScale(new Vector3(7.5f, 7.5f, 7.5f), 0.5f);

        for (int i = 0; i < score - 1; i++)
        {
            calScore++;
        }
        if (calScore >= score)
        {
            calScore = score;
        }
        scoreText.text = "" + calScore * 17f;
    }
}
