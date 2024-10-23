using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject[] npcs;

    int targetLvl = 0;

    void Start()
    {
        ChooseTarget();
        
    }

    public void ChooseTarget()
    {
        if (targetLvl == 5)
        {
            //win
        }
        
        npcs = GameObject.FindGameObjectsWithTag("NPC");

        int randomIndex = Random.Range(0, npcs.Length);
        GameObject target = npcs[randomIndex];
        NPC targetScript = target.GetComponent<NPC>();
        targetScript.Target();

        targetLvl++;
    }

}
