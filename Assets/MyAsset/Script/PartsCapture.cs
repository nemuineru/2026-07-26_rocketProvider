using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsCapture : MonoBehaviour
{
    internal bool isCaptureReady = false;
    internal bool isButtonReleased = false;
    float yPos = 1f;
    float ErasingTime = 0.4f;
    
    [SerializeField]
    int BeatRecorded = 0;

    // [SerializeField]
    // List<GameObject> BeatEffInstance;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isButtonReleased)
        {
            if(isCaptureReady)
            {
                OnReleased();
                isCaptureReady = false;
            }
            ErasingTime -= Time.deltaTime;
        }
        else if(BeatRecorded != GameSystem.self.currentBeatNum)
        {
            int BeatNum = GameSystem.self.currentBeatNum;
            // Instantiate(BeatEffInstance[Mathf.Max(0,BeatNum) % BeatEffInstance.Count], transform.position, Quaternion.identity);
            BeatRecorded = GameSystem.self.currentBeatNum;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * ErasingTime / 0.4f, Time.deltaTime * 5f);
        if(ErasingTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void ReadyCapture()
    {
        Debug.Log("Capture Ready");
        isCaptureReady = true;
        isButtonReleased = false;
        animator.SetBool("isCaptureReady", true);
    }

    public void OnGrabbed()
    {        
        Plane plane =
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ?
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;

        Vector3 Power = (Vector3.Lerp(transform.position, newPosition, .8f) - transform.position) * 24f;
        transform.position += Power * Time.deltaTime;
    }

    //Manufactureにドラッグされた時、そのManfuactureに対しての処理を行う.
    public void OnReleased()
    {
        Plane plane =
        new Plane(GameSystem.self.PartPlane.transform.up, GameSystem.self.PartPlane.transform.position + Vector3.up * yPos);
        Vector3 newPosition = plane.Raycast(GameSystem.self.MainRay, out float distance) ?
        GameSystem.self.MainRay.GetPoint(distance) : transform.position;

        foreach (var man in GameSystem.self.ManufacturerObjects)
        {
            //平面距離で円柱内にいる時
            if
            (Vector3.ProjectOnPlane(newPosition - man.transform.position, Vector3.up).magnitude < man.range ||
            Vector3.ProjectOnPlane(transform.position - man.transform.position, Vector3.up).magnitude < man.range)
            {
                man.BroadcastMessage("Packing", GameSystem.self.CheckBeatInSync(0.3f));
            }
        }
        //animator.SetBool("isCaptureReady", false);
    }
}
