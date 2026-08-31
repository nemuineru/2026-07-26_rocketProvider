using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartMiscUI : MonoBehaviour
{
    Parts part;
    TextMeshPro textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        part = transform.parent.GetComponent<Parts>();
        textMeshPro = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = part.HitPoint.ToString();
        transform.LookAt(Camera.main.transform.position, Vector3.right);
    }
}
