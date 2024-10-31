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
    
    // If no civilians found, return
    if (civilians.Length == 0)
    {
        Debug.LogWarning("No civilians available to target.");
        return;
    }

    GameObject target = null;

    // Loop until a non-null target is found
    for (int attempts = 0; attempts < civilians.Length; attempts++)
    {
        int randomIndex = Random.Range(0, civilians.Length);
        target = civilians[randomIndex];
        
        if (target != null)
        {
            break;
        }
    }

    // If still null, log a warning and return
    if (target == null)
    {
        Debug.LogWarning("No valid target found after multiple attempts.");
        return;
    }

    Civilian targetScript = target.GetComponent<Civilian>();
    
    if (targetScript != null)
    {
        targetScript.target = true;
        pointer.target = target.transform;
    }
    else
    {
        Debug.LogWarning("Target does not have a Civilian script attached.");
    }
}

}
