using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDesc_TutorialUI : MonoBehaviour
{
    [SerializeField]
    public int Selects;
    public List<GameObject> GameObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < GameObjects.Count; i++)
        {
            GameObjects[i].SetActive(i == Selects);
        }
    }
}
