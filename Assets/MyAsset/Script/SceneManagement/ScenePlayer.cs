using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;

public class ScenePlayer : MonoBehaviour
{
    [SerializeField]
    TransitionAnimator transitionsPlus;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float playTime = 0f;

    // Update is called once per frame
    void Update()
    {
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
