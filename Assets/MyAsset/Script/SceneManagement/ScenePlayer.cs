using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;

public class ScenePlayer : MonoBehaviour
{
    [SerializeField]
    TransitionAnimator transitionsPlus;

    [SerializeField]
    GameObject gameInstructSet;

    [SerializeField]
    GameObject InstructionNext, InstructionPrev;

    public MenuType currentMenuType = MenuType.MainGame;

    public enum MenuType
    {
        MainGame,
        Instruction,
        Options,
        Credits
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float playTime = 0f;

    // Update is called once per frame
    void Update()
    {
        
        gameInstructSet.SetActive(currentMenuType == MenuType.Instruction);
        InstructionNext.SetActive(currentMenuType == MenuType.Instruction);
        InstructionPrev.SetActive(currentMenuType == MenuType.Instruction);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Input.GetMouseButtonDown(0))
        {
            transitionsPlus.Play();
        }    
        if( playTime >= transitionsPlus.profile.duration){
            SceneManager.LoadScene("MainGameScene");
        }
        playTime += Time.deltaTime * (transitionsPlus.isPlaying ? 1 : 0);
    }
}
