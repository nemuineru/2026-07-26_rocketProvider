using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GlobalSystem : MonoBehaviour
{
    public static GlobalSystem self;

    // Awake is called when the script instance is being loaded
    // It is used to initialize any variables or game state before the game starts.

    //Language initialization or settings can be added here if needed.
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (self == null)
        {
            self = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //animator = GetComponent<Animator>();
        // for(int i = 0 ; i < MaxParts * dividerNum; i++)
        // {
        //     GameObject segmentPart = 
        //     Instantiate(GameSystem.self.SegmentParts);
        //     SegmentParts.Add(segmentPart);
        // }
    }
}