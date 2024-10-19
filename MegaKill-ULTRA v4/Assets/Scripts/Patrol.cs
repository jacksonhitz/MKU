using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform[] nodes;
    public int target;
    public float spd;

    float gap = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        target = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nodes[target].position, spd * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, nodes[target].position) < gap)
        {
            target++;

            if (target >= nodes.Length)
            {
                target = 0;
            }
        }
        
        
    }
}
