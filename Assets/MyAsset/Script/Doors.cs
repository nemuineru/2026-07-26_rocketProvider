using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MovingObject
{
    [SerializeField]
    Bounds ElementSq;
    public List<Parts> InsideParts = new List<Parts>();

    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        InsideParts.Clear();
        Bounds b = ElementSq;
        b.center = ElementSq.center + transform.position;
        InsideParts = GameSystem.self.parts.FindAll
        (i => i != null && b.Contains(i.transform.position));
    }

    override public void OnMove()
    {
        if(InsideParts.Count >= 3)
        {
            Debug.Log("Door Interacted");
            // foreach(Parts part in InsideParts)
            // {
            //     part.BroadcastMessage("OnMove");
            // }
            //GameSystem.self.CalculateParts(InsideParts);
            base.OnMove();
        }
    }
}
