using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltMovement : MonoBehaviour
{

    Renderer material;
    float CurrentVal = 0f;
    
    [SerializeField]
    float Speed = 0.3f;

    void OnCollisionStay(Collision c)
    {
        Parts pt = c.gameObject.GetComponent<Parts>();
        if(pt != null)
        {
            pt.rb.AddForce(transform.forward * Speed * Time.fixedDeltaTime,ForceMode.VelocityChange);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentVal += Speed * Time.fixedDeltaTime;
        material.material.mainTextureOffset = Vector2.up * CurrentVal;
    }
}
