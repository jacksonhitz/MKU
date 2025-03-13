using UnityEngine;
using System.Collections;

public class NPCAI : MonoBehaviour
{
    public float moveSpeed;
    public Collider moveable;
    public float detectionRadius; 

    Vector3 targetPos;
    bool isWaiting;
    bool isFleeing;

    void Start()
    {
        GameObject moveableObj = GameObject.Find("Moveable");
        moveable = moveableObj.GetComponent<Collider>();

        SetDestination();
    }

    void Update()
    {
        if (isWaiting || isFleeing) return;

        Move();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            StartCoroutine(Wait());
        }
    }

    void SetDestination()
    {
        Bounds bounds = moveable.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        targetPos = new Vector3(randomX, transform.position.y, randomZ);
    }

    IEnumerator Wait()
    {
        isWaiting = true;
        float randomWaitTime = Random.Range(0f, 5f);
        yield return new WaitForSeconds(randomWaitTime);
        SetDestination();
        isWaiting = false;
    }

    void OnDrawGizmosSelected()
    {
        if (moveable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(moveable.bounds.center, moveable.bounds.size);
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void CallFlee(Vector3 playerPos)
    {
        if (!moveable.bounds.Contains(playerPos) || Vector3.Distance(transform.position, playerPos) > detectionRadius)
        {
            return;
        }

        Vector3 fleeDirection = (transform.position - playerPos).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * detectionRadius;

        StopAllCoroutines();
        StartCoroutine(Flee(fleeTarget));
    }

    IEnumerator Flee(Vector3 fleeTarget)
    {
        isFleeing = true;
        Debug.Log("fleeing");
        while (Vector3.Distance(transform.position, fleeTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, fleeTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}