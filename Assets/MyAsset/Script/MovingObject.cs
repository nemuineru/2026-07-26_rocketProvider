using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {        
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    virtual public void OnMove()
    {
        animator.SetTrigger("Moving");
        Debug.Log("Button Interacted");
    }
}
