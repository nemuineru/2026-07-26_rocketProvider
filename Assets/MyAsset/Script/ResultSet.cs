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

    float timeToAwait = 3f; 
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
        int SetScore = 0;
        int SetLevel = 0;
        if(ScoreCapture.Instance != null)
        {
            SetScore = ScoreCapture.Instance.GetScore();
            SetLevel = ScoreCapture.Instance.GetLevel();
        }
        
        if(levelText != null && levelText.IsActive())
        {
            int y = Mathf.RoundToInt(Mathf.Lerp(level, SetLevel, .2f));
            y = y / (float)SetLevel > .95f ? SetLevel : y;
            level = y;
            levelText.text = $"Level {level}";
        }
        if(scoreText != null && scoreText.IsActive())
        {
            int x = Mathf.RoundToInt(Mathf.Lerp(score, SetScore, .2f));
            x = x / (float)SetScore > .95f ? SetScore : x;
            score = x;
            scoreText.text = $"Score\n{score}";
        }

        if(miscText != null && miscText.IsActive() && timeToAwait < 0)
        {
            miscText.text = "click to return Main menu.";
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            }
        }
    }
}
