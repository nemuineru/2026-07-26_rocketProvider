using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class UISystem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI AddingScoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
