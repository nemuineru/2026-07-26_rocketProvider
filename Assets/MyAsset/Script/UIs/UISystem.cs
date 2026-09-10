using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class UISystem : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI nextScoreText;
    public TextMeshProUGUI AddingScoreText;

    public TextMeshProUGUI RhymeChainText;
    public Image GrooveCircleImage;

    float CurrentScore = 0;
    float NextScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrentScore = Mathf.Lerp(CurrentScore, GameSystem.self.Score, .2f);
        NextScore = Mathf.Max(NextScore, GameSystem.self.levelDatas[Mathf.Min(GameSystem.self.Level, GameSystem.self.levelDatas.Count - 1)].nextScore);

        levelText.text = "LEVEL " + GameSystem.self.Level.ToString("D2");
        scoreText.text = "PTS " + ((int)CurrentScore).ToString("D8");
        nextScoreText.text = "NXT " + ((int)NextScore).ToString("D8");

        RhymeChainText.text = GameSystem.self.rhymeChain > 1 ? "Rhyme " + GameSystem.self.rhymeChain.ToString() : "";
        float fills = GameSystem.self.grooveTime / GameSystem.self.grooveTimeMax;
        GrooveCircleImage.fillAmount =  Mathf.Pow(Mathf.Min(fills , 1f), 0.5f);
        RhymeChainText.transform.localScale = Vector3.one * Mathf.Lerp(RhymeChainText.transform.localScale.x, fills > 0.95f ? 1.05f : 1f, .2f);
    }

    public void ShowAddingScore(string score)
    {
        StartCoroutine(DisplayAddingScore(score));
    }

    IEnumerator DisplayAddingScore(string sc)
    {
        int displayIndex = 0;
        int displayIndexMax = 2;
        float displayTime = .5f;
        float elapsedTime = 0f;
        
        List<string> strings = new List<string>();
        //改行で分割してリストに格納
        strings = sc.Split('\n').ToList<string>();
        while(displayIndex < strings.Count)
        {
            //last is longer than usual
            if(displayIndex == strings.Count - 1)
            {
                displayTime = 1.5f;
            }
            //AddingScoreText.text = sc;
            
            // displayTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;
                string displayer = "";
                for(int i = displayIndex; i < strings.Count 
                && i < displayIndex + displayIndexMax; i++)
                {
                    displayer += strings[i] + "\n";
                    // displayIndex++;
                }

                AddingScoreText.text = displayer;
                if(elapsedTime >= displayTime)
                {
                    elapsedTime = 0f;
                    displayIndex += 1;
                }
            yield return null;
        }
        Debug.Log("Ending");
        AddingScoreText.text = "";
    }
}
