

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Manufacture : MonoBehaviour
{
    [SerializeField]
    float GravPower;
    [SerializeField]
    float range;
    [SerializeField]
    float colliderIgnorementRange;

    [SerializeField]
    public GameObject PackagePrefab;
    
    [SerializeField]
    public GameObject UIPrefab;

    bool isColorMirraged = false, isTypeMirraged = false;

    float TypeScore = 0f, colorScore = 0f;

    internal List<Parts> InsideParts = new List<Parts>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(Parts p in InsideParts)
        {
            if(p != null)
            {
                p.SemiGraviTowards(GravPower,range,colliderIgnorementRange);
            }
        }
    }

    //集めたパーツの等分性や統一性を計算し、ポイントとして出す.
    //〇〇 : △△ : ◇◇ で全一色の場合、
    public void CalculateParts(List<Parts> parts)
    {
        float CalcScore = 0f;
        int totalParts = parts.Count;
        // List<Parts> Triangles = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Triangle);
        // List<Parts> Sphere = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Sphere);
        // List<Parts> Box = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Box);

        
        //シェープで分ける.
        var Shapes = parts.GroupBy(pt => pt.PartType).ToList();

        foreach(var shape in Shapes)
        {
            Debug.Log("Shape : " + shape.Key + " Count : " + shape.Count());
        }

        // BonusTriangle = Triangles.Count > 0 ? 1f : 0f;
        // BonusSphere = Sphere.Count > 0 ? 1f : 0f;
        // BonusBox = Box.Count > 0 ? 1f : 0f;

        //トータルパーツが3未満の場合は、スコアを0にする. 
        if( totalParts < 3)
        {
            CalcScore = 0f;
        }
        else
        {
            //パーツ総数（基礎得点）
            float partScore = (float)totalParts;

            
        }
        CalcScore = 100 * parts.Count() * TypeScore * colorScore;
        Debug.Log("CalcScore is " + CalcScore);

    }

    //カラー種類数に応じたボーナス倍率
    void value_Color()
    { 
        List<int> colors = new List<int>();
        foreach(Parts part in InsideParts)
        {
            colors.Add(part.color);
            Debug.Log("col added : " + part.color );
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
        float colorScore;

        bool isColorMirraged = false;
        //色が統一されている場合は、ボーナス点を加算する.
        if(colors.Distinct().Count() == 1)
        {
            Debug.Log("The PURE Archived");
            isColorMirraged = true;
            colorScore = 4f;
        }
        else
        {
            //カラーの個別情報をリスト化.
            // colorNumsはcolorDistinctと同値のサイズを取るはず.
            List<int> colorDistinct = colors.Distinct().ToList();
            List<int> colorNums = new List<int>();
            foreach(int cD in colorDistinct)
            {
                //colorDistinctの値で個数を数える.
                colorNums.Add(colors.Count(keys => keys == cD));
            }

            int MaxColors = colorNums.Max();
            int MinColors = colorNums.Min();
            colorScore = Mathf.Max(1.0f,colorDistinct.Count() * ((float)MinColors / MaxColors));
            //カラーが均一ならボーナス.
            if(MaxColors == MinColors)
            {
                Debug.Log("The E-COLOR Archived");
                colorScore = colorDistinct.Count * MaxColors;
            }
            else
            {
                Debug.Log("MIX Archived");
                colorScore = MinColors;
            }
            Debug.Log("ColorScore is " + colorScore);
        }
    }

    //パーツ統一性ボーナス.
    void value_Types()
    { 
        List<int> colors = new List<int>();

        float TypeScore = 0f;
        //パーツの種類を考慮する.
        List<int> typeCounts = new List<int>();
        bool isTypeMirraged = false;
        foreach(Parts part in InsideParts)
        {
            typeCounts.Add(part.PartType);
        }
        //パーツが統一されているなら、ボーナス. 但し3つ以上で均等に揃えている方が高く付く.
        if(typeCounts.Distinct().Count() == 1)
        {
            Debug.Log("The MIRRAGE Archived");
            isTypeMirraged = true;
            TypeScore = 3f;
        }
        else
        {
            //パーツの個別情報をリスト化.
            // colorNumsはcolorDistinctと同値のサイズを取るはず.
            List<int> partsDistinct = typeCounts.Distinct().ToList();
            List<int> partsNums = new List<int>();
            foreach(int pD in partsDistinct)
            {
                //colorDistinctの値で個数を数える.
                partsNums.Add(colors.Count(keys => keys == pD));
            }

            int MaxParts = partsNums.Max();
            int MinParts = partsNums.Min();
            TypeScore = MinParts / MaxParts;
            //パーツの種類が均等に分布している場合は、ボーナス点を加算する.
            if(MaxParts == MinParts)
            {
                Debug.Log("The DIVISION Archived");
                TypeScore = partsDistinct.Count() * 1.5f;
            }
        }
        Debug.Log("PartsScore is " + TypeScore);
    }

    //

    //特殊ボーナス.
    //▲▲▲ - □□□ となるような組なら、でっかい.
    void MiscBonus()
    {
        List<int> typeCounts = new List<int>();

        bool isGROUPUNITY = true;
        foreach (int p in typeCounts)
        {
            bool isAligned = true;
            List<Parts> pts = InsideParts.Where(o => o.PartType == p).ToList();
            foreach (Parts DivByParts in pts)
            {
                isAligned = pts[0].color == DivByParts.color;
                if (!isAligned)
                {
                    isGROUPUNITY = false;
                    break;
                }
            }
        }
        if (isColorMirraged && isTypeMirraged)
        {
            Debug.Log("The PURE MIRROR Archived");
        }
        else if (isGROUPUNITY)
        {
            Debug.Log("The UNIFIED GROUP Archived");
        }

        //最終的にパーツ数 x シェイプタイプスコア x カラーリング統一性で決定される;
    }
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red * new Color(1,1,1,0.1f);
        Gizmos.DrawSphere(transform.position,range);
        Gizmos.color = Color.cyan * new Color(1,1,1,0.1f);;
        Gizmos.DrawSphere(transform.position,colliderIgnorementRange);
        //Debug.Log("Door Selected");
    }
}

