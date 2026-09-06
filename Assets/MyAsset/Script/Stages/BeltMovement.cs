using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltMovement : MonoBehaviour
{

    Material material;
    float CurrentVal = 0f;
    
    [SerializeField]
    float Speed = 0.3f;
    float calcSpeed = 0f;

    [SerializeField]
    float MaterialLength = 0.3f;

    void OnCollisionStay(Collision c)
    {
        Parts pt = c.gameObject.GetComponent<Parts>();
        if(pt != null)
        {
            pt.rb.MovePosition(pt.rb.position + transform.forward * calcSpeed * Time.fixedDeltaTime);
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
        Speed = GameSystem.self.speed;
        calcSpeed = Mathf.Lerp(calcSpeed, Speed * (GameSystem.self.gameTime > 0f ? 1f : 0.1f),0.1f);
        CurrentVal += calcSpeed * Time.fixedDeltaTime;
        material.mainTextureOffset = Vector2.up * (CurrentVal / MaterialLength);
    }
}
