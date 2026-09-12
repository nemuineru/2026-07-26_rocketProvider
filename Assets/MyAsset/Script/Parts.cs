

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Parts : MonoBehaviour
{
    //Colorは以下として考える.
    //0 = Orange,
    //1 = Magenta,
    //2 = Cyan
    public int color = 0;
    //PartTypeは以下として考える.
    //0 = Orb,
    //1 = Tris,
    //2 = Cube
    public int PartType;
    float yPos = 1f;

    [SerializeField]
    //変換後の配送時間. Splineに沿って、また工場に送られる.
    internal float deliveryTime = 0f;

    public float baseScore = 100f;

    public bool isTrash = false;

    //レベルが上がるに連れ、基本スコアと大きさと重さが変わる.
    public int Level = 1;

    public int HitPoint = 1;

    public int beatStart = 0;

    MeshRenderer mainRenderer;

    MeshFilter mainMeshFilter;
    public Manufacture CapturedBy;
    public Rigidbody rb;
    public Collider collider;

    //梱包完了時のエフェクト
    [SerializeField]
    public GameObject componentEffect;

    //消去時のエフェクト
    [SerializeField]
    public GameObject erasingEffect;

    //パーツのダメージエフェクト
    [SerializeField]
    public GameObject breakingEffect;

    //転がってたりするときのエフェクト
    [SerializeField]
    GameObject GroundEffect;

    //キャプチャーされているときのエフェクト
    [SerializeField]
    GameObject CaptureEffect;


    public bool isGrabbed = false;
    public bool isConsuming = false;
    public bool isDelivered = false;

    public bool isDamaging = false;


    void OnCollisionEnter(Collision collision)
    {
        //Rarely play the touch sound
        if (GameSystem.self.PartSoundOnTouch.Count > 0 && UnityEngine.Random.value > 0.8f)
        {
            AudioSource.PlayClipAtPoint
            (GameSystem.self.PartSoundOnTouch[UnityEngine.Random.Range(0, GameSystem.self.PartSoundOnTouch.Count)], transform.position);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;
        if (GroundEffect != null)
        {
            GroundEffect.transform.position = contactPoint;
            GroundEffect.SetActive(true);
        }
    }

    public void setStatusNum(int color, int partType)
    {
        this.color = color;
        this.PartType = partType;
    }

    public void settingParts()
    {
        mainRenderer.material = GameSystem.self.mats[color];
        mainMeshFilter.mesh = GameSystem.self.partMeshes[PartType];
    } 

    public void init()
    {        
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        mainRenderer = GetComponent<MeshRenderer>();
        mainMeshFilter = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
        if (!isTrash && color >= 0)
        {
            setStatusNum(color, PartType);
            settingParts();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //消費前のパーツはBPMの補正を掛けておく. GameSystem.self.OffsetTimeを使って、音楽の再生位置の補正もかける.
        //減少量に関して、Rhymeコンボ5+になるまでは1->2->3->4->5の順で増加していく.
        if(!isConsuming)
        {
            HitPoint = Level;
            beatStart = GameSystem.self.currentBeatNum;
        }
        else if(isDamaging)
        {
            GameObject effectInstance = Instantiate(componentEffect, transform.position, Quaternion.identity);
            effectInstance.transform.localScale = Vector3.one * 2.0f;
            //大きいほど、HPの減少が遅くなる. つまり、レベルが高いほど、HPの減少が遅くなる.
            //但し、大きくなりすぎないように.
            float DecreaseValue = Mathf.Min(GameSystem.self.rhymeChain + 1, 4);

            //2026-09-12 ..一旦これ減少量を全部にしてテンポ早めたほうが良いかもな―と思ったけどやっぱ却下
            //HitPoint = 0;
            
            //スコア加算は最大値のときのみLV分増加.
            if(HitPoint == Level)
            {
                GameSystem.self.Score += Mathf.RoundToInt(50f * Level);
            }
            //それ以外はBeatごとに.
            else
            {
                GameSystem.self.Score += 100;
            }

            HitPoint -= (int)DecreaseValue;
            GameSystem.self.rhymeChain++;
            //Groovetimeは10LV以上で2beatまで回復. それ以下は1/4beat.
            float GrooveIncreaseValue = Level >= 10 ? 2f : .25f; //Mathf.Min(Level / DecreaseValue , 8f);
            GameSystem.self.grooveTime += GrooveIncreaseValue * GameSystem.self.bpmCaclRate;
    
            float chillingValue = Mathf.Pow(Level, 0.5f) * 2f;

            GameSystem.self.currentLimit -= chillingValue;
            isDamaging = false;
        }
        //キャプチャーされているときは考慮しない.
        collider.enabled = !isGrabbed;
        rb.useGravity = !isGrabbed;
        rb.mass = 1f + Mathf.Pow(Level, 0.25f) * 0.1f;
        //キャプチャーされてたり、配送中の時は小さくする. それ以外はレベルに応じて大きくする.
        //最大5倍スケール.
        float targetScale = Mathf.Min((deliveryTime > 0f || CapturedBy != null) ? 0.7f : (1f + Mathf.Pow(Level, 0.75f) * 0.1f), 8f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.1f);
        if (CapturedBy != null)
        {
            gameObject.layer = LayerMask.NameToLayer("CapturedEntity");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Entity");
        }

        if (CapturedBy != null || deliveryTime > 0f || isDelivered)
        {
            gameObject.layer = LayerMask.NameToLayer("CapturedEntity");
            CaptureEffect.SetActive(true);
            rb.useGravity = false;
            if (deliveryTime > 0f)
            {
                deliveryTime -= Time.deltaTime;
                {
                    float3 pos = GameSystem.self.transportSpline.EvaluatePosition(1 - deliveryTime);
                    Vector3 movePos = new Vector3(pos.x, pos.y, pos.z) - transform.position;
                    rb.MovePosition(Vector3.Lerp(rb.position, transform.position + movePos, 0.1f));
                    rb.velocity =
                    Vector3.Lerp(rb.velocity, movePos, 0.1f);
                }
            }
        }
        else
        {
            CaptureEffect.SetActive(false);
            rb.useGravity = true;
        }
        


        //キャプチャー時や食べさせてる時以外の要因で何らかの原因で溶鉱炉に落ちなかったりした時や、HPが0になった場合は削除する
        if (((transform.position.y < -10f) && CapturedBy == null && deliveryTime <= 0f && !isConsuming) || HitPoint <= 0)
        {
            Deletation(HitPoint > 0);
        }
        if(GameSystem.self.currentGameState == GameSystem.GameState.GameOver)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
        }

    }

    public void Deletation(bool isPenalty = false)
    {
        if (isPenalty)
        {
            GameSystem.self.currentLimit += 5f;
        }
        if (erasingEffect != null)
        {
            GameObject effect = Instantiate(erasingEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        Destroy(gameObject);
    }

    public void OnGrabbed()
    {
        Plane plane =
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ?
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;

        Vector3 Power = (Vector3.Lerp(transform.position, newPosition, .8f) - transform.position) * 24f;
        rb.velocity = Power;
    }

    public void OnReleased()
    {
        CapturedBy = null;

        Debug.Log("Released Part: " + name);
        Plane plane =
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ?
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;

        foreach (var man in GameSystem.self.ManufacturerObjects)
        {
            //平面距離で円柱内にいる時
            if
            (Vector3.ProjectOnPlane(newPosition - man.transform.position, Vector3.up).magnitude < man.range ||
            Vector3.ProjectOnPlane(transform.position - man.transform.position, Vector3.up).magnitude < man.range)
            {
                CapturedBy = man;
                man.InsideParts.Add(this);
            }
        }

        Bounds consumerBoundary = new Bounds(GameSystem.self.consumer.boundary.center, GameSystem.self.consumer.boundary.size);
        consumerBoundary.center += GameSystem.self.consumer.transform.position;

        if(GameSystem.self.consumer != null && GameSystem.self.consumer.collectedParts.Count < GameSystem.self.consumer.MaxParts &&
        (consumerBoundary.ClosestPoint(transform.position) - transform.position).magnitude < 0.1f)
        {
            GameSystem.self.consumer.collectedParts.Add(this);
            isDelivered = true;
        }
    }

    internal void SemiGraviTowards(float power, float nonRange, float ignoreColliders)
    {
        if (CapturedBy == null) return;

        Vector3 Twards = (CapturedBy.transform.position - transform.position);
        float dist = Twards.magnitude;
        collider.enabled = ignoreColliders > dist;

        if (dist > nonRange)
        {
            rb.AddForce(Twards.normalized * (dist - nonRange) * power);
            //外に向かうようならブレーキ.
            if (Vector3.Dot(rb.velocity, Twards.normalized) < 0)
            {
                rb.velocity *= 0.95f; // ブレーキ
            }
        }
    }
}

