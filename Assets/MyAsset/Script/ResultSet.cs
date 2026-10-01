using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultSet : MonoBehaviour
{
    
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    TextMeshProUGUI miscText;

    int score = 0;
    int level = 0;

    float timeToAwait = 5f; 
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "";
        levelText.text = "";
        miscText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        timeToAwait -= Time.deltaTime;
        if(ScoreCapture.Instance != null)
        {
            if(scoreText != null && scoreText.IsActive())
            {
                int SetScore = ScoreCapture.Instance.GetScore();
                score = Mathf.RoundToInt(Mathf.Lerp(score, SetScore, .1f));
                scoreText.text = $"Score\n{score}";
            }
            if(levelText != null && levelText.IsActive())
            {
                int SetLevel = ScoreCapture.Instance.GetLevel();
                level = Mathf.RoundToInt(Mathf.Lerp(level, SetLevel, .1f));
                levelText.text = $"Level {level}";
            }
        }
        if(miscText != null && miscText.IsActive() && timeToAwait < 0)
        {
            miscText.text = "click to return Main menu.";
        }
    }
}
