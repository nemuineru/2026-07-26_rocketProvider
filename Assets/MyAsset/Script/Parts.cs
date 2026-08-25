using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts : MonoBehaviour
{
    //Colorは以下として考える.
    //0 = Orange,
    //1 = Magenta,
    //2 = Cyan
    public int color = 0;
    //PartTypeは以下として考える.
    //0 = Orb,
    //1 = Tris,
    //2 = Cube
    public int PartType;
    float yPos = 1f;

    MeshRenderer rend;
    public Manufacture CapturedBy;
    public Rigidbody rb;
    public Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        rb  = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
        rend.material = GameSystem.self.mats[color];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(CapturedBy != null)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
        if(transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void OnGrabbed()
    {
        Plane plane = new Plane(Vector3.up, Vector3.up * (yPos + 3.0f));
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ? 
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
    }

    internal void SemiGraviTowards(float power, float nonRange, float ignoreColliders)
    {
        Vector3 Twards = (CapturedBy.transform.position - transform.position);
        float dist = Twards.magnitude;
        collider.enabled = ignoreColliders > dist;
        
        if(dist > nonRange)
        {
            rb.AddForce(Twards.normalized * (dist - nonRange) * power);
        }
    }
}
