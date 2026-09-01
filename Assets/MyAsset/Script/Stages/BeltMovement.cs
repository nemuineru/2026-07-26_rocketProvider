using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltMovement : MonoBehaviour
{

    Material material;
    float CurrentVal = 0f;
    
    [SerializeField]
    float Speed = 0.3f;

    [SerializeField]
    float MaterialLength = 0.3f;

    void OnCollisionStay(Collision c)
    {
        Parts pt = c.gameObject.GetComponent<Parts>();
        if(pt != null)
        {
            pt.rb.MovePosition(pt.rb.position + transform.forward * Speed * Time.fixedDeltaTime);
        }
    }
    void OnCollisionExit(Collision c)
    {
        Parts pt = c.gameObject.GetComponent<Parts>();
        if(pt != null)
        {
            pt.rb.AddForce(transform.forward * 1f + transform.up * 1f, ForceMode.VelocityChange);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().materials[1];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentVal += Speed * Time.fixedDeltaTime;
        material.mainTextureOffset = Vector2.up * (CurrentVal / MaterialLength);
    }
}
