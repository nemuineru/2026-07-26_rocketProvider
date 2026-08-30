

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Manufacture : MonoBehaviour
{
    [SerializeField]
    internal float GravPower;
    [SerializeField]
    internal float range;
    [SerializeField]
    internal float colliderIgnorementRange;

    //瓶詰めオブジェクト
    [SerializeField]
    public GameObject PackagePrefab;
    
    [SerializeField]
    public GameObject UIPrefab;

    bool isColorMirraged = false, isTypeMirraged = false;

    float TypeScore = 0f, colorScore = 0f;

    public string calcDesc = "";

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
        InsideParts = InsideParts.Where
        (p => p != null && p.isGrabbed == false).ToList();
    }

    public void Packing()
    {
        if(InsideParts.Count > 2)
        {
            calcDesc = "";
            Debug.Log("Packing initiated");
            Instantiate(PackagePrefab, transform.position - transform.up * 2f, Quaternion.identity);
            CalculateParts(InsideParts);
            //パッケージアニメーションスタート.
            StartCoroutine(PackAnim());
            GameSystem.self.uiSystem.ShowAddingScore(calcDesc);
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
        calcDesc += "Base 100 x" + parts.Count() + '\n';
        colorScore = value_Color();
        TypeScore = value_Types();
        float myscScore = MiscBonus();
        CalcScore = 100 * parts.Count() *  colorScore * TypeScore * myscScore;
        GameSystem.self.Score += (int)CalcScore;

        calcDesc += "Result " + CalcScore;
        Debug.Log("CalcScore is " + CalcScore);
        
    }

    //カラー種類数に応じたボーナス倍率
    float value_Color()
    { 
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
        List<int> colors = new List<int>();
        foreach(Parts part in InsideParts)
        {
            colors.Add(part.color);
            Debug.Log("col added : " + part.color );
        }
        float colorScore;

        isColorMirraged = false;
        //色が統一されている場合は、ボーナス点を加算する.
        if(colors.Distinct().Count() == 1)
        {
            Debug.Log("The PURE Archived");
            calcDesc += "[PURE x4]" + '\n';
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
                calcDesc += string.Format("[E-COLOR x{0}]", colorDistinct.Count * MaxColors) + '\n';
            }
            else
            {
                Debug.Log("MIX Archived");
                colorScore = MinColors;
                calcDesc += string.Format("[MIX x{0}]", MinColors) + '\n';
            }
            Debug.Log("ColorScore is " + colorScore);
        }
        return colorScore;
    }

    //パーツ統一性ボーナス.
    float value_Types()
    { 
        List<int> Types = new List<int>();

        //パーツの種類を考慮する.
        List<int> types = new List<int>();
        foreach(Parts part in InsideParts)
        {
            types.Add(part.PartType);
            Debug.Log("partType added : " + part.PartType );
        }

        float TypeScore = 0f;
        isTypeMirraged = false;

        //パーツが統一されているなら、ボーナス. 但し3つ以上で均等に揃えている方が高く付く.
        if(types.Distinct().Count() == 1)
        {
            Debug.Log("The MIRRAGE Archived");
            calcDesc += "[MIRRAGE x3]" + '\n';
            isTypeMirraged = true;
            TypeScore = 3f;
        }
        else
        {
            //パーツの個別情報をリスト化.
            // colorNumsはcolorDistinctと同値のサイズを取るはず.
            List<int> partsDistinct = types.Distinct().ToList();
            List<int> partsNums = new List<int>();
            foreach(int pD in partsDistinct)
            {
                //colorDistinctの値で個数を数える.
                partsNums.Add(types.Count(keys => keys == pD));
            }

            int MaxParts = partsNums.Max();
            int MinParts = partsNums.Min();
            TypeScore =  Mathf.Max(1.0f,partsDistinct.Count() * ((float)MinParts / MaxParts));
            //パーツの種類が均等に分布している場合は、ボーナス点を加算する.
            if(MaxParts == MinParts)
            {
                Debug.Log("The DIVISION Archived");
                calcDesc += string.Format("[DIVISION x{0}]", partsDistinct.Count() * 1.5f) + '\n';
                TypeScore = partsDistinct.Count() * 1.5f;
            }
            else
            {
                Debug.Log("FRAGMENT Archived");
                TypeScore = MinParts;
                calcDesc += string.Format("[FRAGMENT x{0}]", MinParts) + '\n';
           }
        }
        Debug.Log("PartsScore is " + TypeScore);
        return TypeScore;
    }

    //

    //特殊ボーナス.
    //▲▲▲ - □□□ となるような組なら、でっかい.
    float MiscBonus()
    {
        float myscScore = 1f;
        List<int> types = new List<int>();

        foreach(Parts part in InsideParts)
        {
            types.Add(part.PartType);
            Debug.Log("Group partType added : " + part.PartType );
        }

        //パーツ内のカラーが唯一性を持っているか？
        bool isGROUPUNITY = true;
        foreach (int p in types)
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
            myscScore += 2f; // PURE MIRROR bonus
            calcDesc += string.Format("PURE MIRROR x2]") + '\n';
            Debug.Log("The PURE MIRROR Archived");
        }
        else if (isGROUPUNITY)
        {
            myscScore += .5f * InsideParts.Count; // UNIFIED GROUP bonus
            calcDesc += string.Format("UNIFIED GROUP x{0}]", myscScore) + '\n';
            Debug.Log("The UNIFIED GROUP Archived");
        }

        //最終的にパーツ数 x シェイプタイプスコア x カラーリング統一性で決定される;
        return myscScore;
    }
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red * new Color(1,1,1,0.1f);
        Gizmos.DrawSphere(transform.position,range);
        Gizmos.color = Color.cyan * new Color(1,1,1,0.1f);;
        Gizmos.DrawSphere(transform.position,colliderIgnorementRange);
        //Debug.Log("Door Selected");
    }

    IEnumerator PackAnim()
    {
        Debug.Log("Packanim Init");
        float dulation = 1.3f;
        float elapsed = 0f;
        //スクリプト記述によるアニメーション.
        while(elapsed < dulation)
        {
            foreach(Parts part in InsideParts)
            {
                // パーツを現在の移動方向からパッケージコンポーネントの
                // 渦上に移動させる処理をここに記述する.
                Vector3 direction = (transform.position - part.transform.position).normalized;
                Vector3 plane = Vector3.Cross(direction, transform.up);
                Vector3 toCenter = Vector3.ProjectOnPlane(direction, transform.up);
                Vector3 dir = (plane.normalized * (1 - (elapsed / dulation)) +
                 (toCenter.normalized * (elapsed / dulation) * 40.0f) - transform.up) * 0.2f;
                Debug.DrawLine(part.transform.position, part.transform.position + dir * 20f, Color.green);
                part.rb.velocity += (dir);
                part.transform.localScale *=  Mathf.Pow(1.0f - elapsed / dulation, 0.1f);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        foreach(Parts part in InsideParts)
        {
            Destroy(part.gameObject);
        }
    }
}

