using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartMiscUI : MonoBehaviour
{
    Parts part;
    TextMeshPro textMeshPro;

    [SerializeField]
    bool isHitPointUI = true;

    bool isFlickering = false;
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
        if(part.Level >= 4)
        {
            if(!isFlickering)
            {
                textMeshPro.color = new Color(0.7f, 0.4f, 0.3f);
            }
            else
            {
                textMeshPro.color = Color.white;
            }
        }
        if(part.isConsuming)
        {
            if(!isFlickering)
            {
                textMeshPro.color = new Color(0.9f, 0.4f, 0.3f);
            }
            else
            {
                textMeshPro.color = Color.red;
            }
        }
        isFlickering = !isFlickering;
        transform.LookAt(Camera.main.transform.position, Vector3.right);
    }
}
