

using UnityEngine;
using System.Collections.Generic;

//これが生成されている間に、GameSystemに何らかの影響を与える.
//継承用クラス.
class UI_EventShow : MonoBehaviour
{
    public float duration = 1.0f;
    public float elapsedTime = 0.0f;
    public string eventText = "Event";

    void Start()
    {
        //UIのテキストを設定する.
        UnityEngine.UI.Text text = this.GetComponentInChildren<UnityEngine.UI.Text>();
        if (text != null)
        {
            text.text = eventText;
            //メインキャンバスに乗せるため、transformを移動する.
            Canvas mainCanvas = GameObject.FindObjectOfType<Canvas>();
            if (mainCanvas != null)
            {
                this.transform.SetParent(mainCanvas.transform, false);
                
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
        {
            Destroy(this.gameObject);
        }
    }
}

