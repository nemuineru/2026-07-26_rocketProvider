using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCapture : MonoBehaviour
{
    public static ScoreCapture Instance;
    int Score = 0;
    int Level = 0;
    public int GetScore() { return Score; }
    public int GetLevel() { return Level; }

    // Start is called before the first frame update
    // Static instance initialization. Ensures only one instance of ScoreCapture exists.
    // And also prevents this object from being destroyed when loading a new scene.
    void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    // Get the latest score and level from the GameSystem
    void Update()
    {
        if(GameSystem.self != null)
        {
            Score = GameSystem.self.Score;
            Level = GameSystem.self.Level;
        }
    }
}
