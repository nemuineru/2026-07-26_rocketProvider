using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInstance : MonoBehaviour
{
    static public InputInstance self;
    InputMaster inputMaster;
    public Vector2 position;
    public bool isClicked;
    public int clickingTime;
    void Awake()
    {
        if (self != null)
        {
            Destroy(self);
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
    void Start()
    {
        
    }

    // Update is called once per frame
    public void InputUpdate()
    {
        position = inputMaster.Basic.ScreenPosition.ReadValue<Vector2>();
        isClicked = inputMaster.Basic.Click.ReadValue<float>() > 0.5f;
        clickingTime = isClicked ? clickingTime + 1 : 0;
        // Vector3 pos = Camera.main.ScreenToWorldPoint(position);
    }
}
