using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInstance : MonoBehaviour
{
    static public InputInstance self;
    InputMaster inputMaster;
    public Vector2 position;
    public bool isClicked;
    public bool isFastforwarded;
    public int clickingTime;
    public int escapePressedTime;

    //keep the instance alive across scenes.
    //if another instance exists, destroy it to maintain a single instance.
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (self != null)
        {
            Destroy(gameObject);
        }
        else
        {
            self = this;
        }
        //set the inputmaster online.
        inputMaster = new InputMaster();
        inputMaster.Enable();
    }

    // Start is called before the first frame update
    void FixedUpdate()
    {
        escapePressedTime = inputMaster.Basic.Escape.ReadValue<float>() > 0.5f ? escapePressedTime + 1 : 0;
    }

    // Update is called once per frame
    public void InputUpdate()
    {
        if(GameSystem.self != null && GameSystem.self.inDemomode != true)
        {
            position = inputMaster.Basic.ScreenPosition.ReadValue<Vector2>();
            isClicked = inputMaster.Basic.Click.ReadValue<float>() > 0.5f;
            isFastforwarded = inputMaster.Basic.FastForward.ReadValue<float>() > 0.5f;
            clickingTime = isClicked ? clickingTime + 1 : 0;
        // Vector3 pos = Camera.main.ScreenToWorldPoint(position);
        }
    }
}
