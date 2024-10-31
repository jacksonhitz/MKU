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

    public Pointer pointer;
    public CamController cam;

    public Material camMat;
    float setLerp = 0.5f;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        volume = FindAnyObjectByType<Volume>();
        ux = FindAnyObjectByType<UX>();

        ChooseTarget();
    }

    public void UpPhase()
    {
        phase++;
        cam.UpPhase(phase);

        setLerp = (setLerp * phase);

        camMat.SetFloat("_Lerp", setLerp);

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
        Debug.Log("civs: " + civilians.Length);
        
        int randomIndex = Random.Range(0, civilians.Length);
        GameObject target = civilians[randomIndex];

        if (target == null)
        {
            ChooseTarget();
        }

        Civilian targetScript = target.GetComponent<Civilian>();
        
        targetScript.target = true;
        pointer.target = target.transform;
    }
    
}
