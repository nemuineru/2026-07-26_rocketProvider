using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    //必ず１つは付ける.
    public Material[] mats;

    [SerializeField]
    static public GameSystem self;

    public List<Parts> parts;

    public Parts grabbingParts;

    public Ray MainRay;

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
        parts = parts.Where(i => i != null).ToList();
        if(InputInstance.self.isClicked)
        {
            Debug.Log("Click");
            if(grabbingParts == null)
            {
                GetParts();
            }
            if(grabbingParts != null)
            {
                grabbingParts.isGrabbed = true;
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
        MainRay = new Ray(pos,mainCam.transform.forward);
    }
    
    void GetParts()
    {
        LayerMask lMask = LayerMask.GetMask("Entity");
        bool isHit = Physics.Raycast(MainRay,out RaycastHit hitInfo,lMask);

        Debug.Log("Hit : " + isHit);

        if(isHit)
        {
            Parts HitPart = hitInfo.collider.gameObject.GetComponent<Parts>();
            if(HitPart != null)
            {
                grabbingParts = HitPart;
                Debug.Log("Hit Part : " + HitPart.name);
                Debug.Log("Hit Part Layer : " + LayerMask.LayerToName(HitPart.gameObject.layer));
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
