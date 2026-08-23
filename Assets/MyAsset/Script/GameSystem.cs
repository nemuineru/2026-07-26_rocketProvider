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
        InputInstance.self.InputUpdate();

        parts = parts.Where(i => i != null).ToList();
        if(InputInstance.self.isClicked)
        {
            //Debug.Log("Click");
            if(grabbingParts == null)
            {
                GetParts();
                //押した瞬間、ボタン押しの判定にする.
                if(grabbingParts == null && InputInstance.self.clickingTime == 1)
                {
                    PushButton();
                }
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

        //Debug.Log("Hit : " + isHit);

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

    //集めたパーツの等分性や統一性を計算し、ポイントとして出す.
    //〇〇 : △△ : ◇◇ で全一色の場合、
    public void CalculateParts(List<Parts> parts)
    {
        float CalcScore = 0f;
        int totalParts = parts.Count;
        List<Parts> Triangles = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Triangle);
        List<Parts> Sphere = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Sphere);
        List<Parts> Box = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Box);

        
        //シェープで分ける.
        var Shapes = parts.GroupBy(pt => pt.pType).ToList();

        foreach(var shape in Shapes)
        {
            Debug.Log("Shape : " + shape.Key + " Count : " + shape.Count());
        }

        // BonusTriangle = Triangles.Count > 0 ? 1f : 0f;
        // BonusSphere = Sphere.Count > 0 ? 1f : 0f;
        // BonusBox = Box.Count > 0 ? 1f : 0f;

        List<int> colors = new List<int>();
        foreach(Parts part in parts)
        {
            colors.Add(part.color);
        }
        //トータルパーツが3未満の場合は、スコアを0にする. 
        if( totalParts < 3)
        {
            CalcScore = 0f;
        }
        else
        {
            //パーツ総数（基礎得点）
            float partScore = (float)totalParts;
            //カラー種類数
            float colorScore;
            //色が統一されている場合は、ボーナス点を加算する.
            if(colors.Distinct().Count() == 1)
            {
                colorScore = 4f;
            }
            /*
            1,1,2,2,3

            -> 偏り性を計算するとき..
            2/5,[2]/5,[1]/5.
            カラー倍数 : 1/2倍.

            1,1,2,2,2,3

            -> 偏り性を計算するとき
            2/6, [3]/6,[1]/6
            カラー倍数 : 1/3..となる

            */
            else
            {
                List<int> colorCounts = new List<int>();
                //カラーの種類ごとの個数
                foreach(int color in colors)
                {
                    int index = colors.IndexOf(color);
                    if(index == -1)
                    {
                        colorCounts.Add(color);
                    }
                    else
                    {
                        colorCounts[index] += 1;
                    }
                }
                //カラーが均等に分布している場合は、ボーナス点を加算する. 
                if(colorCounts.Max() == colorCounts.Min())
                {
                    colorScore = 2f;
                }
                else
                {
                    colorScore = colorCounts.Min() / colorCounts.Max();
                }
            }


            float TypeScore = 0f;
            //パーツの種類が均等に分布している場合は、ボーナス点を加算する.
            List<int> typeCounts = new List<int>();
            foreach(Parts part in parts)
            {
                int index = typeCounts.IndexOf((int)part.pType);
                if(index == -1)
                {
                    typeCounts.Add((int)part.pType);
                }
                else
                {
                    typeCounts[index] += 1;
                }
            }
            //パーツの種類が均等に分布している場合は、ボーナス点を加算する.
            if(typeCounts.Max() == typeCounts.Min())
            {
                // ここにボーナス点の計算ロジックを追加
            }
        }

    }

}
