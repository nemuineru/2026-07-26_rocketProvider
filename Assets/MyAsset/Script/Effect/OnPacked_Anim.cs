using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class OnPacked_Anim : MonoBehaviour
{
    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    MeshFilter mainMeshFilter;

    internal int partsColor, partsType;
    
    Vector3 initVect = Vector3.up + Vector3.forward;
    float throwingTime_Max = .3f;
    float throwingTime = 1f;
    float deletionTime_Max = 1f;
    float deletionTime = 1f;

    internal SplineContainer splineContainer;

    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        deletionTime = deletionTime_Max;
        throwingTime = throwingTime_Max;
        meshRenderer.material = GameSystem.self.mats[partsColor];
        mainMeshFilter.mesh = GameSystem.self.partMeshes[partsType];
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += initVect * throwingTime * Time.deltaTime;
        float t = Mathf.Pow(Mathf.Clamp01(throwingTime / throwingTime_Max), 4);
        float f = Mathf.Clamp01(deletionTime / deletionTime_Max);
        transform.position = Vector3.Lerp( transform.position, splineContainer.EvaluatePosition(1 - f),1 - t);
        throwingTime -= Time.deltaTime;
        if(throwingTime <= 0f)
        {
            deletionTime -= Time.deltaTime;
            if(deletionTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
