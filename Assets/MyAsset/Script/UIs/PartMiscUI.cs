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
    // Start is called before the first frame update
    void Start()
    {
        part = transform.parent.GetComponent<Parts>();
        textMeshPro = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHitPointUI)
        {
            textMeshPro.text = part.HitPoint.ToString();
        }
        else if(part.isConsuming)
        {
            textMeshPro.text = Mathf.RoundToInt((part.HitPoint / 1.0f) * part.Level).ToString();
        }
        //普通のパーツのレベルは1なので、レベルが1の時は表示しない.
        else
        {
            textMeshPro.text = part.Level <= 2 ? "" : part.Level.ToString();
        }
        transform.LookAt(Camera.main.transform.position, Vector3.right);
    }
}
