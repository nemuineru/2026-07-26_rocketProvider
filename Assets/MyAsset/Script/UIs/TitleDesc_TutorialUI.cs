using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDesc_TutorialUI : MonoBehaviour
{
    [SerializeField]
    public int Selects;
    [SerializeField]
    TMPro.TextMeshProUGUI PrevText, NextText;
    public List<GameObject> GameObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        PrevText.text = "<< モドル";
        NextText.text = ">> ススム";
        if(Selects == 0)
        {
            PrevText.text = "<< スタート";
        }
        else if(Selects == GameObjects.Count - 1)
        {
            NextText.text = ">> ワカッタ";
        }
        if(Selects < 0 || Selects >= GameObjects.Count)
        {
            Selects = 0;
            ScenePlayer.instance.currentMenuType = ScenePlayer.MenuType.MainGame;
        }
        
        for(int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].SetActive(i == Selects);
        }
    }

    public void SetIncrement(int increment)
    {
        Selects += increment;
    }
}
