using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetSubUI : MonoBehaviour
{
    [SerializeField]
    CinemachineBrain cinemachineBrain;
    [SerializeField]
    CinemachineVirtualCamera subCamera;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
        subCamera.Priority = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
