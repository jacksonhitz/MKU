using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    GameObject[] civilians;

    public int phase;

    public Volume volume;

    public int score = 0;

    public UX ux;

    public SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        volume = FindAnyObjectByType<Volume>();
        ux = FindAnyObjectByType<UX>();

        UpPhase();
    }

    public void UpPhase()
    {
        phase++;
        //volume.weight = phase * .2f;
        soundManager.Stop();
        soundManager.PhaseCheck();
    }

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;
        
        ux.Score();
        ux.PopUp(newScore);
    }

    public void ChooseTarget()
    {
        civilians = GameObject.FindGameObjectsWithTag("Civilian");
        
        for (int i = 0; i < 25; i++)
        {
            int randomIndex = Random.Range(0, civilians.Length);
            GameObject target = civilians[randomIndex];
            Civilian targetScript = target.GetComponent<Civilian>();

            if (targetScript.target)
            {
                i--;
            }
            else
            {
                targetScript.target = true;
                Debug.Log("targets+1");
            }
        }
        Debug.Log("npcs: " + civilians.Length);
    }

}
