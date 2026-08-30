using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageComps : MonoBehaviour
{
    [SerializeField]
    float time = 1.3f;
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Package Sealed");
            Destroy(gameObject);
        }
    }
}