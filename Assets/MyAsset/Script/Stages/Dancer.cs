

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dancer : MonoBehaviour
{
    Animator animator;
    int BeatRecorded = 0;
    int RandomPose;
    [SerializeField]
    int Randomrange;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameSystem.self != null)
        {
            OnBeatUpdation();
            StatusSet();
        }
    }

    void OnBeatUpdation()
    {
        if (GameSystem.self.rhymeChain > 0 && GameSystem.self.rhymeChain % 4 == 1 && BeatRecorded != GameSystem.self.currentBeatNum) // Assuming a 4-beat cycle for the dancer
        {
            animator.SetTrigger("OnBeat");
            BeatRecorded = GameSystem.self.currentBeatNum;
            RandomPose = Random.Range(0, Randomrange); // Assuming 4 different poses
        }
    }

    void StatusSet()
    {
        bool isIdle = GameSystem.self.dangerCount >= 10f && GameSystem.self.rhymeChain == 0;
        bool isDanger = !isIdle && GameSystem.self.rhymeChain == 0;
        bool isGameOver = GameSystem.self.dangerCount <= 0f;
        animator.SetBool("OnIdle", isIdle);
        animator.SetBool("OnDanger", isDanger);
        animator.SetBool("OnGameOver", isGameOver);
        animator.SetInteger("RandomPose", RandomPose);
    }

    void OnDanger()
    {
    }
}

