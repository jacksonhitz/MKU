using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    GameObject[] npcs;

    public int phase;

    public Volume volume;

    void Start()
    {
        ChooseTarget();
    }


    public void ChooseTarget()
    {
        phase++;
        
        volume.weight = (phase * 1f);
        
        npcs = GameObject.FindGameObjectsWithTag("NPC");

        Debug.Log("npcs: " + npcs.Length);

        int randomIndex = Random.Range(0, npcs.Length);
        GameObject target = npcs[randomIndex];
        NPC targetScript = target.GetComponent<NPC>();
        targetScript.target = true;
    }

}
