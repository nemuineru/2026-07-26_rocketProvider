using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : PhysicalInteractable
{
    Animator animator;
    [SerializeField] MovingObject movingObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator = GetComponent<Animator>();
    }

    override public void OnInteract()
    {
        animator.SetTrigger("Press");
        movingObject.BroadcastMessage("OnMove");
        Debug.Log("Button Interacted");
    }
}
