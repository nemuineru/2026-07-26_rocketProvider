using UnityEngine;
using UnityEngine.Splines;
using Cinemachine;

/// <summary>
/// スプラインを使ったカメラの移動経路
/// </summary>
public class MySplineDolly : CinemachinePathBase
{
    // カメラの移動経路（スプライン）
    [SerializeField] private SplineContainer _spline;

    // パスのローカル空間における位置
    public override Vector3 EvaluateLocalPosition(float pos)
    {
        return _spline.Spline.EvaluatePosition(pos);
    }

    // パスのローカル空間における傾き
    public override Vector3 EvaluateLocalTangent(float pos)
    {
        return _spline.Spline.EvaluateTangent(pos);
    }

    // パスのローカル空間における向き
    public override Quaternion EvaluateLocalOrientation(float pos)
    {
        return Quaternion.LookRotation(
            EvaluateLocalTangent(pos),
            _spline.Spline.EvaluateUpVector(pos)
        );
    }

    // パスの位置の最小値
    public override float MinPos => 0;

    // パスの位置の最大値
    public override float MaxPos => 1;

    // パスはループしているかどうか
    public override bool Looped => _spline.Spline.Closed;

    // パスの解像度
    public override int DistanceCacheSampleStepsPerSegment => m_Resolution;

    // 直近位置の検索
    public override float FindClosestPoint(Vector3 p, int startSegment, int searchRadius, int stepsPerSegment)
    {
        // パスのローカル空間における位置に変換
        p = _spline.transform.InverseTransformPoint(p);
        
        // スプラインの最近接点を求める
        SplineUtility.GetNearestPoint(
            _spline.Spline,
            p,
            out _,
            out var t
        );

        return t;
    }
}