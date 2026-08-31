using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LimitUI : MonoBehaviour
{
    public UnityEngine.UI.RawImage limitImage;

    [SerializeField]
    float YMin = 0f;
    [SerializeField]
    float YMax = 5f;
    public TextMeshProUGUI limitText;
    float limitRate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        limitRate = Mathf.Lerp(limitRate, GameSystem.self.currentLimit / GameSystem.self.maxLimit, Time.deltaTime * 5f);
        float yPos = Mathf.Lerp(YMin, YMax, limitRate);
        float xPos = Time.time * (limitRate + 1.0f);
        float width = Mathf.Lerp(3f, .8f, limitRate);
        limitImage.uvRect = new Rect(xPos, yPos, width, limitImage.uvRect.height);
        limitText.text = "LIMIT" + "\n" + (limitRate * 100f).ToString("F1") + "%";
    }
}
