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

    internal List<Parts> parts = new List<Parts>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(Parts p in parts)
        {
            p.SemiGraviTowards(GravPower,range,colliderIgnorementRange);
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
            List<int> colors = new List<int>();
            foreach(Parts part in parts)
            {
                colors.Add(part.color);
                Debug.Log("col added : " + part.color );
            }
            
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
                //カラーが均一なら2倍ボーナス.
                if(MaxColors == MinColors)
                {
                    colorScore = 2.0f;
                }
                Debug.Log("ColorScore is " + colorScore);
            }


            float TypeScore = 0f;
            //パーツの種類を考慮する.
            List<int> typeCounts = new List<int>();
            foreach(Parts part in parts)
            {
                typeCounts.Add(part.PartType);
            }
            //パーツが統一されているなら、ボーナス. 但し3つ以上で均等に揃えている方が高く付く.
            if(typeCounts.Distinct().Count() == 1)
            {
                TypeScore = 2f;
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
                    TypeScore = partsDistinct.Count();
                }
            }
            Debug.Log("PartsScore is " + TypeScore);
            //最終的にパーツ数 x シェイプタイプスコア x カラーリング統一性で決定される;
            CalcScore = 100 * parts.Count() * TypeScore * colorScore;
        }
        Debug.Log("CalcScore is " + CalcScore);

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
