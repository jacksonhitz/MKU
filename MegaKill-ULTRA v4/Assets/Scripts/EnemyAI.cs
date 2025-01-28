using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject player;
    public float detectionRadius = 10f;
    public int numRays = 8;
    public float moveSpeed = 5f;

    Vector3 targetPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Pathfind();
        Move();
    }

    void Pathfind()
    {
        Vector3 closestPoint = Vector3.zero;
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i < numRays; i++)
        {
            float angle = (360f / numRays) * i;
            Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, detectionRadius))
            {
                float distanceToPlayer = Vector3.Distance(hit.point, player.transform.position);
                if (distanceToPlayer < shortestDistance)
                {
                    shortestDistance = distanceToPlayer;
                    closestPoint = hit.point;
                }
            }
        }

        if (closestPoint == Vector3.zero)
        {
            closestPoint = player.transform.position;
        }

        targetPosition = closestPoint;
    }

    void Move()
    {
        if (targetPosition != Vector3.zero)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
