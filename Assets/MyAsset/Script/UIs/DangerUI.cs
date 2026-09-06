using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DangerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI dangerIndicator;
    [SerializeField]
    private TextMeshProUGUI dangerText;

    [SerializeField]
    private Image fukidashi;

    int DangerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DangerCount != Mathf.CeilToInt(GameSystem.self.dangerCount))
        {
            StartCoroutine(SpinningCount());
            DangerCount = Mathf.CeilToInt(GameSystem.self.dangerCount);
        }
        if(GameSystem.self.currentLimit >= 100)
        {
            dangerIndicator.text = DangerCount.ToString();
        }
        if(GameSystem.self.currentLimit >= GameSystem.self.maxLimit)
        {
            dangerText.text = "DANGER";
        }
        else
        {
            dangerText.text = "";
        }
        dangerIndicator.text = GameSystem.self.dangerCount < 10 ? DangerCount.ToString() : "";
        fukidashi.enabled = GameSystem.self.dangerCount < 10;
    }

    IEnumerator SpinningCount()
    {
        float m_times = .2f;
        float times = m_times;
        while(times > 0)
        {
            dangerIndicator.transform.Rotate(Vector3.up * 360f * Mathf.Sin(Mathf.PI * times / m_times * .5f));
            yield return null;
            times -= Time.deltaTime;
        }
        dangerIndicator.transform.rotation = Quaternion.identity;
    }
}
