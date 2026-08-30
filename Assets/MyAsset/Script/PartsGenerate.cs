using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PartsGenerate : MonoBehaviour
{
    [SerializeField]
    public Bounds bounds;
    public List<GameObject> lists;

    [SerializeField]
    float freqency = 10;
    float timerMax = 0.32f;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timerMax = Mathf.Min(100f , 1.0f / (freqency + Mathf.Epsilon));
        if(timer > timerMax)
        {
            GameSystem.self.parts.Add(Generate());
            timer = 0.0f;
        }
        timer += Time.fixedDeltaTime;
    }

    Parts Generate()
    {
        GameObject instObj = lists[Random.Range(0,lists.Count)];
        Vector3 pos = transform.position + bounds.center + new Vector3
        ( Random.Range(-bounds.extents.x,bounds.extents.x),
          Random.Range(-bounds.extents.y,bounds.extents.y),
          Random.Range(-bounds.extents.z,bounds.extents.z));
         GameObject sels = Instantiate(instObj, pos, Quaternion.identity);
         Parts part = sels.GetComponent<Parts>();
         part.color = Random.Range(0,GameSystem.self.mats.Count());
         return part;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube( transform.position + bounds.center, 
        new Vector3(bounds.extents.x * 2, bounds.extents.y * 2, bounds.extents.z * 2));
    }
}
