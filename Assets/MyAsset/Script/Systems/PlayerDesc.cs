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
