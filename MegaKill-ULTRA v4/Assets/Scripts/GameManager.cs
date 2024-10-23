using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject[] npcs;

    void Start()
    {
        ChooseTarget();
        
    }

    public void ChooseTarget()
    {
        npcs = GameObject.FindGameObjectsWithTag("NPC");

        int randomIndex = Random.Range(0, npcs.Length);
        GameObject target = npcs[randomIndex];
        NPC targetScript = target.GetComponent<NPC>();
        targetScript.Target();
    }

}
