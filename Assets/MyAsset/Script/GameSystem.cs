using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameSystem : MonoBehaviour
{
    //必ず１つは付ける.
    public Material[] mats;

    [SerializeField]
    static public GameSystem self;

    public List<Parts> parts;

    public Parts grabbingParts;

    public Ray MainRay;

    public List<Manufacture> ManufacturerObjects;

    //Static化
    void Awake()
    {
        if(GameSystem.self != null)
        {
            Destroy(gameObject);
        }
        else
        {
            self = this;
        }
    }

    void mouseSelect()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetRay();
        DebugView();
        InputInstance.self.InputUpdate();

        parts = parts.Where(i => i != null).ToList();
        if(InputInstance.self.isClicked)
        {
            //Debug.Log("Click");
            if(grabbingParts == null)
            {
                //押した瞬間、ボタン押しの判定にする.
                if(grabbingParts == null && InputInstance.self.clickingTime == 1)
                {
                    GetParts(0);
                    PushButton();
                }
            }
        }
        else
        {
            grabbingParts = null;
        }
    }
    

    void SetRay()
    {
        Camera mainCam = Camera.main;

        Vector3 StPos = Camera.main.transform.position;
        Vector3 pos = Camera.main.ScreenToWorldPoint(InputInstance.self.position); 
        if(mainCam.orthographic == true)
        {
            MainRay = new Ray(pos,mainCam.transform.forward);
        }
        else
        {
            MainRay = mainCam.ScreenPointToRay(InputInstance.self.position);
        }
    }
    
    void GetParts(int SetTransformIndex)
    {
        LayerMask lMask = LayerMask.GetMask("Entity");
        Manufacture f = ManufacturerObjects[SetTransformIndex];
        bool isHit = Physics.Raycast(MainRay,out RaycastHit hitInfo,lMask);

        //Debug.Log("Hit : " + isHit);

        if(isHit)
        {
            Parts HitPart = hitInfo.collider.gameObject.GetComponent<Parts>();
            if(HitPart != null)
            {
                parts.Add(HitPart);
                HitPart.CapturedBy = f;
                f.InsideParts.Add(HitPart);
                Debug.Log("Hit Part : " + HitPart.name);
                Debug.Log("Hit Part Layer : " + LayerMask.LayerToName(HitPart.gameObject.layer));
            }
        }
    }

    void PushButton()
    {
        LayerMask lMask = LayerMask.GetMask("Button");
        bool isHit = Physics.Raycast(MainRay,out RaycastHit hitInfo,lMask);

        //Debug.Log("Hit : " + isHit);

        if(isHit)
        {
            PhysicalInteractable interactable = hitInfo.collider.gameObject.GetComponent<PhysicalInteractable>();
            if(interactable != null)
            {
                interactable.SendMessage("OnInteract");
                Debug.Log("Hit Button : " + interactable.name);
                Debug.Log("Hit Button Layer : " + LayerMask.LayerToName(interactable.gameObject.layer));
            }
        }
    }

    

    void DebugView()
    {
        Camera mainCam = Camera.main;

        Debug.DrawLine(MainRay.origin, MainRay.origin + MainRay.direction * 100f, Color.red);
        Debug.DrawLine(MainRay.origin, MainRay.origin + Vector3.up, Color.red);
    }

}
