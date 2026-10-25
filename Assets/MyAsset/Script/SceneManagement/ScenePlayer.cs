using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;
using UnityEngine.Localization.Settings;

public class ScenePlayer : MonoBehaviour
{
    public enum LocaleType
    {
        Japanese,
        English
    }

    public LocaleType currentLocale = LocaleType.Japanese;

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

    public async void SetLocale(string enumLocale)
    {
        currentLocale = (LocaleType)System.Enum.Parse(typeof(LocaleType), enumLocale);
        var locale = LocalizationSettings.AvailableLocales.Locales.Find((x) => x.Identifier.Code == enumLocale);

        // Localeの設定
        LocalizationSettings.SelectedLocale = locale;

        // 初期化待ち
        await LocalizationSettings.InitializationOperation.Task;
    
    }
}
