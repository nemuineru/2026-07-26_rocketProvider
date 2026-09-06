using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Splines;

public class GameSystem : MonoBehaviour
{
    //高レベルパーツ生成用のゲームオブジェクト
    public GameObject PartPurePrefab;
    public AudioSource IngameAudio;

    //必ず１つは付ける.
    public Material[] mats;
    //必ず１つは付ける.
    public Mesh[] partMeshes;

    public GameObject SegmentParts;

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

    public AudioSource LevelUpSound;

    public Consumer consumer;

    //現在の経過時間, 及びにレベル.
    public float timeElapsed;

    public int Level = 1;

    //スコアと進行スピード.
    public int Score;

    //流している音楽のパラメータ. 及びにグルーヴ状態の管理.
    public float tempo;
    public float audioOffset;
    
    
    public int rhymeChain = 0;

    //イキオイ状態の時間. これが0になるとコンボボーナスが切れる.
    public float grooveTime;
    public float grooveTimeMax = 4.0f;

    //次のレベルに上がるためのスコアの閾値.
    public int NextLevelScore = 1000;

    public float speed = 1.0f;
    public float generatingRate = 1.0f;

    //currentLimitが100となったあと、デンジャーカウントが始まる.
    //デンジャーカウントが0になるとゲームオーバー. 一度でも100以下になれば徐々にデンジャーカウントは減少する.
    internal float maxLimit = 100f;
    public float minLimit = 0f;
    public float currentLimit = 0f;

    float dangerCountMax = 10;
    public float dangerCount = 0;


    public float gameTime = -2.8f;

    public UISystem uiSystem;

    public List<LevelData> levelDatas;

    [SerializeField]
    //工場配送・出荷ラインの指定
    internal SplineContainer transportSpline;

    public float bpmCaclRate = 1.0f;
    public float exactMusicTime = 1.0f;

    public int currentBeatNum;


    [System.Serializable]
    public class LevelData
    {
        //レベルの最小スタート地点
        public int startLevel;

        //ベルトの進むスピード
        public float speed;

        //パーツ生成レート
        public float generatingRate;

        //自然減少するリミット値レート
        public float limitDecreasingRate;

        //見逃し減少値の倍数値
        public float missedDecreaseMultiplier;

        //生成されるパーツのリスト
        public List<Parts> generatingParts;

        public LevelData(float speed, float generatingRate)
        {
            this.speed = speed;
            this.generatingRate = generatingRate;
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
        currentLimit = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        bpmCaclRate = 60f / tempo;
        currentBeatNum = Mathf.CeilToInt((exactMusicTime - audioOffset) / (60f / tempo));
        float levelProgression = 10000f;
        //3000点毎にレベルアップ.
        int newLevel = 1 + Mathf.FloorToInt(Score / levelProgression);
        if(newLevel > Level)
        {
            LevelUpSound.Play();
        }
        Level = newLevel;

        gameTime += Time.fixedDeltaTime;
        //ゲーム時間が0以上の時のみ、ゲームシステムを動かす.
        if(gameTime > 0f)
        {
            if(IngameAudio != null && !IngameAudio.isPlaying)
            {
                IngameAudio.Play();
            }
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
            grooveTime -= Time.fixedDeltaTime * bpmCaclRate;
            grooveTime = Mathf.Clamp(grooveTime, 0f, grooveTimeMax);
            
            rhymeChain = grooveTime > 0f ? rhymeChain : 0;
            //時間経過で緩やかに.
            currentLimit += Time.fixedDeltaTime * Level * .25f;
            currentLimit = Mathf.Clamp(currentLimit, minLimit, maxLimit);
            //現在の再生位置の正確な時間を取得.
            exactMusicTime = (float)IngameAudio.timeSamples / IngameAudio.clip.frequency;
        }

        if(currentLimit >= maxLimit)
        {
            dangerCount -= Time.fixedDeltaTime / bpmCaclRate;
        }
        else
        {
            dangerCount += Time.fixedDeltaTime / bpmCaclRate * .5f;
            dangerCount = Mathf.Clamp(dangerCount, 0f, 10f);
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
                //配送中のモノはグラブできない. 使用中のも同様.
                if (HitPart.deliveryTime > 0f || HitPart.isConsuming) return;
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

