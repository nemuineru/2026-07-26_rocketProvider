using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Consumer : MonoBehaviour
{
    
    //格納可能な最大パーツ数.
    [SerializeField]
    internal int MaxParts;

    public List<Parts> collectedParts;

    public List<GameObject> SegmentParts;

    [SerializeField]
    SplineContainer splineContainer;

    public int SetBeatAt = 2;

    Animator animator;

    int dividerNum = 3;

    int beatErased = -4;

    [SerializeField]
    GameObject Gateways;

    [SerializeField]
    public Bounds boundary;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundary.center + transform.position, boundary.size);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        for(int i = 0 ; i < MaxParts * dividerNum; i++)
        {
            GameObject segmentPart = 
            Instantiate(GameSystem.self.SegmentParts);
            SegmentParts.Add(segmentPart);
        }
    }

    void FixedUpdate()
    {

        int beatMatch = 
            GameSystem.self.currentBeatNum;

        //bool canErase = beatErased <= beatMatch - 2;        

        int index_Reverse = MaxParts * dividerNum;
        foreach(var part in SegmentParts)
        {
            float t = (float)index_Reverse / (float)(MaxParts * dividerNum);
            if(part != null)
            {
                part.transform.position = splineContainer.EvaluatePosition(t);
                part.transform.localScale = index_Reverse % dividerNum == 0 ? Vector3.one : Vector3.one * 0.5f;
            }
            index_Reverse--;
        }
        //消費したパーツはリストから削除する.
        collectedParts.RemoveAll(item => item == null);
        if(collectedParts.Count > 0)
        {
            int index_Parts = 0;
            foreach(var part in collectedParts)
            {
                Vector3 HeadPos = splineContainer.EvaluatePosition(((float)index_Parts + 1f) / collectedParts.Count);
                if(index_Parts == 0)
                {
                    part.isConsuming = true;
                    if(part.beatStart < beatMatch && beatMatch % SetBeatAt == 0)
                    {
                        part.isDamaging = true;
                        part.beatStart = beatMatch;
                        beatErased = beatMatch;
                    }
                    //音楽のBPMに合わせて、消費するパーツのHPを増減. BPMが早いほど、HP消費は早くなる. 及びにスコアを加算する. 音楽再生位置の補正もかける.
                    float baseDecreasement = GameSystem.self.tempo / 60f * Time.fixedDeltaTime;
                    HeadPos = transform.position + Vector3.up * 8f;
                }
                else
                {
                    part.isDamaging = false;
                }
                part.transform.position = Vector3.Lerp(part.transform.position, HeadPos, 0.5f);
                part.transform.localScale = Vector3.one * 1f;
                part.rb.velocity = Vector3.Lerp(part.rb.velocity, Vector3.zero, 0.5f);
                index_Parts++;
            }
        }
        bool isGatewayActive = collectedParts.Count >= MaxParts;
        if(Gateways != null)
        {
            Gateways.SetActive(isGatewayActive);
        }
    }
}