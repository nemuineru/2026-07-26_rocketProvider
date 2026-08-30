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

    [SerializeField]
    GameObject CaptureEffect;


    public bool isGrabbed = false;

    void OnCollisionEnter(Collision collision)
    {
        if(GameSystem.self.PartSoundOnTouch.Count > 0)
        {
            AudioSource.PlayClipAtPoint
            (GameSystem.self.PartSoundOnTouch[Random.Range(0, GameSystem.self.PartSoundOnTouch.Count)], transform.position);
        }
    }

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
        //キャプチャーされているときは考慮しない.
        collider.enabled = !isGrabbed;
        rb.useGravity = !isGrabbed;
        if(CapturedBy != null)
        {
            gameObject.layer = LayerMask.NameToLayer("CapturedEntity");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Entity");
        }

        if(CapturedBy != null)
        {
            CaptureEffect.SetActive(true);
            rb.useGravity = false;
        }
        else
        {
            CaptureEffect.SetActive(false);
            rb.useGravity = true;
        }
        if(transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    public void OnGrabbed()
    {
        Plane plane = 
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ? 
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;
        
        Vector3 Power = (Vector3.Lerp(transform.position , newPosition, .8f) - transform.position) * 24f;
        rb.velocity = Power;
    }

    public void OnReleased()
    {
        CapturedBy = null;
        
        Debug.Log("Released Part: " + name);
        Plane plane = 
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ? 
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;
        
        foreach(var man in GameSystem.self.ManufacturerObjects)
        {
            if
            ((newPosition - man.transform.position).magnitude < man.range || 
            (transform.position - man.transform.position).magnitude < man.range)
            {
                CapturedBy = man;
                man.InsideParts.Add(this);
            }
        }
    }

    internal void SemiGraviTowards(float power, float nonRange, float ignoreColliders)
    {
        if(CapturedBy == null) return;

        Vector3 Twards = (CapturedBy.transform.position - transform.position);
        float dist = Twards.magnitude;
        collider.enabled = ignoreColliders > dist;
        
        if(dist > nonRange)
        {
            rb.AddForce(Twards.normalized * (dist - nonRange) * power);
            //外に向かうようならブレーキ.
            if(Vector3.Dot(rb.velocity, Twards.normalized) < 0)
            {
                rb.velocity *= 0.95f; // ブレーキ
            }
        }
    }
}
