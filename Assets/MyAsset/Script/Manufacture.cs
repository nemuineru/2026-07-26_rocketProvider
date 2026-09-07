

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
    public GameObject NormalPackage, CriticalPackage;
    
    [SerializeField]
    public GameObject UIPrefab;

    //保持可能なパーツの最大数.
    [SerializeField]
    public int MaxParts = 10;

    bool isColorMirraged = false, isTypeMirraged = false;

    float typeLevel = 0f, colorLevel = 0f;

    public string calcDesc = "";

    bool isPacking = false;

    //何拍で圧縮を実行するかを決定.
    //４拍ごとに、パッケージング.
    int BeatTo = 4;
    int BeatOffSet = 0;
    int BeatRecorded = 0;

    internal List<Parts> InsideParts = new List<Parts>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Parts p in InsideParts)
        {
            if (p != null)
            {
                p.SemiGraviTowards(GravPower, range, colliderIgnorementRange);
            }
        }
        InsideParts = InsideParts.Where
        (p => p != null && p.isGrabbed == false).ToList();
        // // Packing is triggered every BeatTo beats.
        // if (GameSystem.self != null && (GameSystem.self.currentBeatNum - BeatOffSet) % BeatTo == 0)
        // {
        //     Packing();
        // }
    }

    //圧縮再配送スクリプト
    public void Packing(bool isCritical = false)
    {
        Instantiate(isCritical ? CriticalPackage : NormalPackage, transform.position - transform.up * 2f, Quaternion.identity);
        if (InsideParts.Count > 0 && !isPacking && BeatRecorded != GameSystem.self.currentBeatNum)
        {
            calcDesc = "";
            Debug.Log("Packing initiated");
            isPacking = true;
            BeatRecorded = GameSystem.self.currentBeatNum;
            Parts resultPart;
            GameObject instPart;
            resultPart = CalculateParts(InsideParts, isCritical);
            //パッケージアニメーションスタート.
            StartCoroutine(PackAnim(resultPart));
            GameSystem.self.uiSystem.ShowAddingScore(calcDesc);
        }
    }
    
    //出荷スクリプト
    public void Shipping()
    {
        if (InsideParts.Count > 0)
        {
            foreach (Parts p in InsideParts)
            {
                p.deliveryTime = 1f;
            }
            InsideParts.Clear();
        }
    }

    //集めたパーツの等分性や統一性を計算し、ポイントとして出す.
    //〇〇 : △△ : ◇◇ で全一色の場合、☆型の統一された色で排出され、LVはボーナスを加味して10程.
    //返り値として生成されたパーツを返すように変更.
    public Parts CalculateParts(List<Parts> parts, bool isCritical = false)
    {
        Parts resultPart = new Parts();
        float CalcLevel = 0f;
        int totalParts = parts.Count;
        // List<Parts> Triangles = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Triangle);
        // List<Parts> Sphere = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Sphere);
        // List<Parts> Box = (List<Parts>)parts.Where(pt => pt.pType == Parts.PartType.Box);

        int mostPart = 0;
        int mostColor = 0;

        //シェープで分ける.
        var Shapes = parts.GroupBy(pt => pt.PartType).ToList();

        foreach (var shape in Shapes)
        {
            Debug.Log("Shape : " + shape.Key + " Count : " + shape.Count());
        }

        // BonusTriangle = Triangles.Count > 0 ? 1f : 0f;
        // BonusSphere = Sphere.Count > 0 ? 1f : 0f;
        // BonusBox = Box.Count > 0 ? 1f : 0f;

        //トータルパーツが3未満の場合は、スコアを0にする. 
        if (totalParts < 3)
        {
            calcDesc = "スクナイ..";
            CalcLevel = 0f;
        }
        else
        {
            float partBaseLevel = (float)parts.Average(p => p.Level);
            calcDesc += "ベースレベル " + partBaseLevel + '\n';
            (colorLevel, mostColor) = value_Color();
            (typeLevel, mostPart) = value_Types();
            float myscLevel = MiscBonus();
            //成長させるレベルの計算.
            CalcLevel = (partBaseLevel + colorLevel + typeLevel) * myscLevel;

            //GameSystem.self.Score += (int)CalcScore;
        }

        //複製させる.
        // GameObject gameObject =
        // Instantiate(resultPart.gameObject);

        var SelectPart = parts.Find(o => o.PartType == mostPart);
        
        //レベル1以上であれば、リザルトパーツを生成する.
        if(CalcLevel > 0)
        {
            //resultPartの一部パラメータをSelectPartのパラメータに合わせる.
            resultPart.baseScore = SelectPart.baseScore;
            resultPart.HitPoint = SelectPart.HitPoint;
            resultPart.Level = Mathf.CeilToInt(CalcLevel) + (isCritical ? 2 : 0);
            resultPart.PartType = mostPart;
            resultPart.color = mostColor;
        }
        //そうじゃないときは爆発させる
        else
        {
            
        }

        calcDesc += "リザルト " + CalcLevel;
        Debug.Log("CalcLevel is " + CalcLevel);
        return resultPart;
    }

    //カラー種類数に応じたボーナス倍率
    (float, int) value_Color()
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
            if(part.isTrash)
            {
                continue;
            }
            colors.Add(part.color);
            Debug.Log("col added : " + part.color );
        }
        float colorScore = 1f;
        int selectColor = 0;

        isColorMirraged = false;
        //色が統一されている場合は、ボーナス点を加算する.
        if(colors.Distinct().Count() == 1)
        {
            Debug.Log("The PURE Archived");
            calcDesc += "[オソロイ!! +4]" + '\n';
            isColorMirraged = true;
            colorScore = 4f;
            selectColor = colors[0];
        }
        else if(colors.Count() > 0)
        {
            //カラーの個別情報をリスト化.
            // colorNumsはcolorDistinctと同値のサイズを取るはず.
            List<int> colorDistinct = colors.Distinct().ToList();
            List<int> colorNums = new List<int>();
            foreach (int cD in colorDistinct)
            {
                //colorDistinctの値で個数を数える.
                colorNums.Add(colors.Count(key => key == cD));
            }

            int MaxColors = colorNums.Max();
            int MinColors = colorNums.Min();
            colorScore = Mathf.Max(1.0f,colorDistinct.Count() * ((float)MinColors / MaxColors));
            int selCol = colorNums.IndexOf(colorNums.Max());
            Debug.Log("MaxColor " + selCol.ToString());
            selectColor = colorDistinct[selCol];
            //カラーが均一ならボーナス.
            if(MaxColors == MinColors)
            {
                Debug.Log("The E-COLOR Archived");
                colorScore = colorDistinct.Count * MaxColors;
                calcDesc += string.Format("[イロイロ! +{0}]", colorDistinct.Count * MaxColors) + '\n';
            }
            else
            {
                Debug.Log("MIX Archived");
                colorScore = MinColors;
                calcDesc += string.Format("[バラバラ +{0}]", MinColors) + '\n';
            }
            Debug.Log("ColorScore is " + colorScore);
        }
        return (colorScore, selectColor);
    }

    //パーツ統一性ボーナス.
    (float, int) value_Types()
    { 
        List<int> Types = new List<int>();

        //パーツの種類を考慮する.
        List<int> types = new List<int>();
        foreach(Parts part in InsideParts)
        {
            if(part.isTrash)
            {
                continue;
            }
            types.Add(part.PartType);
            Debug.Log("partType added : " + part.PartType );
        }

        float TypeScore = 0f;
        int selectType = 0;
        isTypeMirraged = false;

        //パーツが統一されているなら、ボーナス. 但し3つ以上で均等に揃えている方が高く付く.
        if(types.Distinct().Count() == 1)
        {
            Debug.Log("The MIRRAGE Archived");
            calcDesc += "[ソックリ!! +3]" + '\n';
            isTypeMirraged = true;
            selectType = types[0];
            TypeScore = 3f;
        }
        else if(types.Count() > 0)
        {
            //パーツの個別情報をリスト化.
            // colorNumsはcolorDistinctと同値のサイズを取るはず.
            List<int> partsDistinct = types.Distinct().ToList();
            List<int> partsNums = new List<int>();
            foreach(int pD in partsDistinct)
            {
                //colorDistinctの値で個数を数える.
                partsNums.Add(types.Count(key => key == pD));
            }
            //最大個数のパーツを選択する.
            selectType = partsDistinct[partsNums.IndexOf(partsNums.Max())];
            

            int MaxParts = partsNums.Max();
            int MinParts = partsNums.Min();
            TypeScore =  Mathf.Max(1.0f,partsDistinct.Count() * ((float)MinParts / MaxParts));
            //パーツの種類が均等に分布している場合は、ボーナス点を加算する.
            if(MaxParts == MinParts)
            {
                Debug.Log("The DIVISION Archived");
                calcDesc += string.Format("[キッチリ! +{0}]", partsDistinct.Count() * 1.5f) + '\n';
                TypeScore = partsDistinct.Count() * 1.5f;
            }
            else
            {
                Debug.Log("FRAGMENT Archived");
                TypeScore = MinParts;
                calcDesc += string.Format("[フゾロイ +{0}]", MinParts) + '\n';
            }
        }
        Debug.Log("PartsScore is " + TypeScore);
        return (TypeScore, selectType);
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
            if(part.isTrash || part.PartType < 0 || part.color < 0)
            {
                continue;
            }
            types.Add(part.PartType);
            Debug.Log("Group partType added : " + part.PartType );
        }

        //パーツ内のカラーが唯一性を持っているか？
        bool isGROUPUNITY = types.Count() > 0;
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
            calcDesc += string.Format("トウイツ?! x2]") + '\n';
            Debug.Log("The PURE MIRROR Archived");
        }
        else if (isGROUPUNITY)
        {
            myscScore += .5f * InsideParts.Count; // UNIFIED GROUP bonus
            calcDesc += string.Format("ハッピー!!! x{0}]", myscScore) + '\n';
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

    IEnumerator PackAnim(Parts resultPart)
    {
        Debug.Log("Packanim Init");
        float dulation = 1.3f;
        float elapsed = 0f;
        List<Parts> partsToPack = new List<Parts>(InsideParts);
        //スクリプト記述によるアニメーション.
        while(elapsed < dulation)
        {
            foreach(Parts part in partsToPack)
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
        foreach(Parts part in partsToPack)
        {
            Destroy(part.gameObject);
        }
        GameObject instPart = Instantiate(GameSystem.self.PartPurePrefab, transform.position, Quaternion.identity);

        Parts parInstPart = instPart.GetComponent<Parts>();
        parInstPart.init();
        Debug.Log("Result Part : " + resultPart.color + " : " + resultPart.PartType + " : " + resultPart.Level);
        parInstPart.color = resultPart.color;
        parInstPart.PartType = resultPart.PartType;
        parInstPart.Level = resultPart.Level;
        parInstPart.settingParts();
        parInstPart.deliveryTime = 1f;

        isPacking = false;
    }
}

