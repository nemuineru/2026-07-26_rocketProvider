using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//set settings for each players. I need to define and change GameSystem related settings for each players. 
//This class will hold player-specific configurations and allow for easy modification of player attributes within the game system.
public class PlayerDesc : MonoBehaviour
{
    // Player-specific attributes. Netcode for GameObject needs to implement..
    public int playerID;
    public string playerName;

    // Player's holding Viewing Axis. We'll use this as manipulation of Hands.
    public Vector3 playerViewingAxis;
    
    // Player's PointInput. Needs to be usable at demo..
    public Vector3 playerPointInputPos;
    public bool isGrabInput;
    int grabbedTime;

    public bool isFastForwarded;
    int fastForwardedTime;

    //public bool 


    // Player-specific score and limit values.
    public float playerScore;

    // limit Max and Current for the player. min is 0.
    public float playerLimitMax;
    public float playerLimitCurrent;
    public float playerDangerCount;
    public float playerDangerCountMax = 10;
    
    public float missedDecreaseMultiplier = 1.0f;
    public float EndTime;


    //what character this player is using..
    public CharacterData characterData;

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
