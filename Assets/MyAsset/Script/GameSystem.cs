using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Splines;

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

    //partの動きのための平面
    public GameObject PartPlane;

    public List<AudioClip> PartSoundOnTouch;
    public List<AudioClip> PartSoundOnSelected;

    //現在の経過時間, 及びにレベル.
    public float timeElapsed;

    public int Level = 1;

    //スコアと進行スピード.
    public int Score;

    //次のレベルに上がるためのスコアの閾値.
    public int NextLevelScore = 1000;

    public float speed = 1.0f;
    public float generatingRate = 1.0f;

    //currentLimitがmaxLimitを超えたらゲームオーバーにする.
    internal float maxLimit = 100f;
    public float currentLimit = 0f;

    //イキオイ状態の時間. これが0になるとコンボボーナスが切れる.
    public float comboTime = 0f;

    public UISystem uiSystem;

    public List<LevelData> levelDatas;

    //工場配送・出荷ラインの指定
    internal SplineContainer transportSpline, shippingSpline;

    [System.Serializable]
    public class LevelData
    {
        public int startLevel;
        public float speed;
        public float generatingRate;
        public float maxLimit;
        public List<Parts> generatingParts;

        public LevelData(float speed, float generatingRate, float maxLimit)
        {
            this.speed = speed;
            this.generatingRate = generatingRate;
            this.maxLimit = maxLimit;
        }
    }

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

    void mouseSgelect()
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
        if (InputInstance.self.isClicked)
        {
            //Debug.Log("Click");
            if (grabbingParts != null)
            {
                grabbingParts.isGrabbed = true;
                grabbingParts.OnGrabbed();
            }

            //押した瞬間、ボタン押しの判定にする. また、パーツのグラブ判定もここで発生させる.
            if (grabbingParts == null && InputInstance.self.clickingTime == 1)
            {
                GrabParts();
                PushButton();
            }
        }
        else
        {
            if (grabbingParts != null)
            {
                grabbingParts.OnReleased();
                grabbingParts.isGrabbed = false;
                grabbingParts = null;
            }
        }
        //0以上の時 コンボボーナスの時間を減らす.
        comboTime -= comboTime > 0 ? Time.deltaTime : 0f;
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
    
    void GrabParts()
    {
        LayerMask lMask = LayerMask.GetMask("Entity") + LayerMask.GetMask("CapturedEntity");
        bool isHit = Physics.SphereCast(MainRay.origin, 0.5f, MainRay.direction, out RaycastHit hitInfo, 50f , lMask);

        //Debug.Log("Hit : " + isHit);

        if(isHit)
        {
            Debug.Log("Functioning Gathering  - " + hitInfo.collider.gameObject.name);
            Parts HitPart = hitInfo.collider.gameObject.GetComponent<Parts>();
            if(HitPart != null)
            {
                //得点源にならないものをクリックした時はHPを減らす.
                if (HitPart.isTrash)
                {
                    HitPart.HitPoint--;
                    Instantiate(HitPart.breakingEffect, HitPart.transform.position, Quaternion.identity);
                    return;
                }
                grabbingParts = HitPart;
                Debug.Log("Hit Part : " + HitPart.name);
                Debug.Log("Hit Part Layer : " + LayerMask.LayerToName(HitPart.gameObject.layer));
            }
            
            if(GameSystem.self.PartSoundOnSelected.Count > 0)
            {
                AudioSource.PlayClipAtPoint
                (GameSystem.self.PartSoundOnSelected[Random.Range(0, GameSystem.self.PartSoundOnSelected.Count)], transform.position);
            }
        }
    }

    void PushButton()
    {
        LayerMask aMask = LayerMask.GetMask("Button");
        bool hitInfos = Physics.Raycast(MainRay,out RaycastHit hitInfo_PB, 50f , aMask);


        if(hitInfos)
        {
            Debug.Log("Functioning PushButton - " + hitInfo_PB.collider.gameObject.name);
            InteractableButton interactable = hitInfo_PB.collider.gameObject.GetComponent<InteractableButton>();
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

