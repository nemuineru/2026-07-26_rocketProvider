using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    // Character-specific attributes
    public string characterName;
    public int characterID;

    // I'll define it as Dancer. 
    public GameObject characterPrefab;

    // Character's audio voide clip. On erase or On Attack-Chaining.. etc.
    public AudioClip voiceClip;

    //Part Damage Effect definitions.
    [SerializeField]
    public GameObject breakingEffect;

    //Part Erasing Complete Effect.
    [SerializeField]
    public GameObject erasingEffect;

    //On Detonate Ready Effect.
    [SerializeField]
    public GameObject partEraseReadyEff;

    //On Missed Effect.
    [SerializeField]
    public GameObject partMissedEff;
    
    //転がってたりするときのエフェクト
    [SerializeField]
    public GameObject partGroundEffect;

    //キャプチャーされているときのエフェクト
    [SerializeField]
    public GameObject partCaptureEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
