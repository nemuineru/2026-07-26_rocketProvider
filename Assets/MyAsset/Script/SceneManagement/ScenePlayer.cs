using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;

public class ScenePlayer : MonoBehaviour
{
    static public ScenePlayer instance;

    [SerializeField]
    TransitionAnimator transitionsPlus;

    [SerializeField]
    GameObject gameInstructSet, gameTitleSet;

    public MenuType currentMenuType = MenuType.MainGame;

    public string LoadSceneName;

    public enum MenuType
    {
        MainGame,
        Instruction,
        Options,
        Credits
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    float playTime = 0f;

    // Update is called once per frame
    void Update()
    {
        gameInstructSet.SetActive(currentMenuType == MenuType.Instruction);
        gameTitleSet.SetActive(currentMenuType == MenuType.MainGame);

        if(InputInstance.self.escapePressedTime == 1)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        if( playTime >= transitionsPlus.profile.duration){
            SceneManager.LoadScene(LoadSceneName);
        }
        playTime += Time.deltaTime * (transitionsPlus.isPlaying ? 1 : 0);
    }

    public void StartTransition(string sceneName)
    {
        LoadSceneName = sceneName;
        transitionsPlus.Play();
    }

    public void SetMenuType(string menuType)
    {
        currentMenuType = (MenuType)System.Enum.Parse(typeof(MenuType), menuType);
    }
}
