using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : PhysicalInteractable
{
    Animator animator;
    [SerializeField] Manufacture manufacture;
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
        if(manufacture != null)
        {
            manufacture.BroadcastMessage("Packing");            
        }
        Debug.Log("Button Interacted");
    }
}
