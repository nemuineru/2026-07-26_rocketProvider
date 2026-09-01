


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

    //変換後の配送時間. Splineに沿って、また工場に送られる.
    internal float deliveryTime = 0f;

    public float baseScore = 100f;

    public bool isTrash = false;

    //レベルが上がるに連れ、基本スコアと大きさと重さが変わる.
    public int Level = 1;

    public int HitPoint = 1;

    MeshRenderer rend;
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


    void OnCollisionEnter(Collision collision)
    {
        if (GameSystem.self.PartSoundOnTouch.Count > 0)
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
        if (!isTrash && color >= 0)
        {
            rend.material = GameSystem.self.mats[color];
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //キャプチャーされているときは考慮しない.
        collider.enabled = !isGrabbed;
        rb.useGravity = !isGrabbed;
        rb.mass = 1f + Level * 0.4f;
        transform.localScale = Vector3.one * (1f + Level * 0.08f);
        if (CapturedBy != null)
        {
            gameObject.layer = LayerMask.NameToLayer("CapturedEntity");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Entity");
        }

        if (CapturedBy != null || deliveryTime > 0f)
        {
            gameObject.layer = LayerMask.NameToLayer("CapturedEntity");
            CaptureEffect.SetActive(true);
            rb.useGravity = false;
            if (deliveryTime > 0f)
            {
                deliveryTime -= Time.deltaTime;
                {
                    GameSystem.self.transportSpline[0].Evaluate(deliveryTime,out float3 pos, out float3 tangent, out float3 upVect);
                    Vector3 movePos = new Vector3(pos.x, pos.y, pos.z) - transform.position;
                    rb.velocity =
                    Vector3.Lerp(rb.velocity, movePos.normalized * 10f, 0.1f);
                }
            }
        }
        else
        {
            CaptureEffect.SetActive(false);
            rb.useGravity = true;
        }
        


        //何らかの原因で溶鉱炉に落ちなかったり、HPが0になった場合は削除する.
        if (transform.position.y < -10f || HitPoint <= 0)
        {
            GameSystem.self.currentLimit += 1f;
            Deletation();
        }

    }

    public void Deletation()
    {
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
            if
            ((newPosition - man.transform.position).magnitude < man.range ||
            (transform.position - man.transform.position).magnitude < man.range)
            {
                CapturedBy = man;
                man.InsideParts.Add(this);
            }
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